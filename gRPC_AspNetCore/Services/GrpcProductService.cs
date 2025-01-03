using Azure.Core;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPC_AspNetCore.Context;
using gRPC_AspNetCore.Models;
using gRPC_AspNetCore.Protos;
using Microsoft.EntityFrameworkCore;
using static gRPC_AspNetCore.Protos.ProductService;

namespace gRPC_AspNetCore.Services
{
    public class GrpcProductService(GrpcContext ctx):ProductServiceBase
    {
        public override async Task CreateProduct(IAsyncStreamReader<CreateProductRequest> requestStream, IServerStreamWriter<CreateProductReply> responseStream, ServerCallContext context)
        {
            int createdProductsCount = 0;
            while(await requestStream.MoveNext())
            {
                ctx.Products.Add(new Product
                {
                    Title = requestStream.Current.Title,
                    Description = requestStream.Current.Description,
                    Price = requestStream.Current.Price,
                    CreateDate = DateTime.Now
                });

                createdProductsCount++;
            }

            await ctx.SaveChangesAsync();

            await responseStream.WriteAsync(new CreateProductReply
            {
                CreatedItemsCount = createdProductsCount,
                Message = "Products created successfully",
                Status = 200
            });
        }

        public override async Task<GetProductByIdReply> GetProductById(GetProductByIdRequest request, ServerCallContext context)
        {
            Product? product = await ctx.Products.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (product is null)
                return new GetProductByIdReply();

            return new GetProductByIdReply
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CreateDate = Timestamp.FromDateTime(product.CreateDate)
            };
        }

        public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<GetAllProductsReply> responseStream, ServerCallContext context)
        {
            int skip = request.Take * request.Page;

            List<Product> products = await ctx.Products
                                              .Skip(skip)
                                              .Take(request.Take)
                                              .ToListAsync();

            foreach(var product in products)
            {
                await responseStream.WriteAsync(new GetAllProductsReply
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    CreateDate = Timestamp.FromDateTime(product.CreateDate)
                });
            }
        }

        public override async Task<RemoveProductByIdReply> RemoveProductById(IAsyncStreamReader<RemoveProductByIdRequest> requestStream, ServerCallContext context)
        {
            int removeItemsCount = 0;

            while (await requestStream.MoveNext())
            {
                Product? product = await ctx.Products.FirstOrDefaultAsync(p => p.Id == requestStream.Current.Id);

                if (product is null)
                    continue;

                ctx.Products.Remove(product);

                removeItemsCount++;
            }
            await ctx.SaveChangesAsync();

            return new RemoveProductByIdReply
            {
                Message = "Remove Product Successfully Done",
                RemovedItemsCount = removeItemsCount,
                Status = 200
            };

        }

        public override async Task<UpdateProductReply> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            Product? product = await ctx.Products.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (product is null)
            {
                // return null;
                throw new NullReferenceException();
            }

            product.Title = request.Title;
            product.Description = request.Description;
            product.Price = request.Price;

            ctx.Products.Update(product);
            await ctx.SaveChangesAsync();

            return new UpdateProductReply
            {
                Message = "Update Product Successfully Done",
                Status = 200,
                UpdatedItemsCount = 1
            };
        }
    }
}
