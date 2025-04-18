﻿using Microsoft.EntityFrameworkCore;
using TechGear.OrderService.Data;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Models;
using TechGear.ProductService.DTOs;

namespace TechGear.OrderService.Services
{
    public class CartService(TechGearOrderServiceContext context, IHttpClientFactory httpClientFactory) : ICartService
    {
        private readonly TechGearOrderServiceContext _context = context;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<CartItemDto>?> GetAllCartItemsByUserId(int userId)
        {
            var cartItems = await _context.CartItems.Include(c => c.Cart)
                .Where(c => c.Cart.UserId == userId)
                .ToListAsync();

            var productItemIds = cartItems.Select(c => c.ProductItemId).ToList();

            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var response = await client.PostAsJsonAsync("api/v1/ProductItems/get-by-ids", productItemIds);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var productItemsInfo = await response.Content.ReadFromJsonAsync<List<ProductItemInfoDto>>();

            var results = cartItems.Select(ci =>
            {
                var info = productItemsInfo?.FirstOrDefault(p => p.ProductItemId == ci.ProductItemId);

                return new CartItemDto
                {
                    ProductItemId = ci.ProductItemId,
                    Quantity = ci.Quantity,
                    ProductName = info?.ProductName ?? "",
                    Sku = info?.Sku ?? "",
                    ImageUrl = info?.ImageUrl ?? "",
                    Price = info?.Price ?? 0
                };
            });

            return results;
        }

        public async Task<IEnumerable<CartItemDto>?> GetAllCartItemsByIds(List<int> productItemIds)
        {
            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var response = await client.PostAsJsonAsync("api/v1/ProductItems/get-by-ids", productItemIds);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var productItemsInfo = await response.Content.ReadFromJsonAsync<List<ProductItemInfoDto>>();

            var result = productItemsInfo?.Select(p => new CartItemDto
            {
                ProductItemId = p.ProductItemId,
                Sku = p.Sku,
                ProductName = p.ProductName,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
            });

            return result;
        }

        public async Task<bool> AddListToCartAsync(int userId, List<int>? productItemIds)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }

            foreach (var productItemId in productItemIds!)
            {
                var existingItem = cart?.CartItems
                    .FirstOrDefault(ci => ci.ProductItemId == productItemId);

                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        CartId = cart!.Id,
                        ProductItemId = productItemId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(newCartItem);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AddToCartAsync(int userId, int productItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductItemId = productItemId,
                    Quantity = 1
                };
                _context.CartItems.Add(newCartItem);
            }
            else
            {
                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductItemId == productItemId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductItemId = productItemId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(newCartItem);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int userId, int productItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductItemId == productItemId);
            if (itemToRemove == null) return false;

            _context.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int userId, int productItemId, int quantity)
        {
            if (quantity <= 0) return await DeleteAsync(userId, productItemId);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var itemToUpdate = cart.CartItems.FirstOrDefault(ci => ci.ProductItemId == productItemId);
            if (itemToUpdate == null) return false;

            itemToUpdate.Quantity = quantity;
            _context.CartItems.Update(itemToUpdate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
