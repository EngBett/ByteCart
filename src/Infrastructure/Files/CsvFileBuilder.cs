using System.Text;
using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Products.Models;

namespace ByteCart.Infrastructure.Files;

public class CsvFileBuilder : ICsvFileBuilder
{
    public byte[] BuildProductsFile(IEnumerable<ProductDto> products)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Id,Name,SKU,Description,Price,StockQuantity,Status,CategoryNames,TagNames,SupplierName,CreatedAt");

        foreach (var product in products)
        {
            stringBuilder.AppendLine(
                $"{Escape(product.Id)},{Escape(product.Name)},{Escape(product.SKU)},{Escape(product.Description)},{product.Price},{product.StockQuantity},{Escape(product.Status)},{Escape(string.Join(";", product.CategoryNames))},{Escape(string.Join(";", product.TagNames))},{Escape(product.SupplierId)},{product.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        return Encoding.UTF8.GetBytes(stringBuilder.ToString());
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        // Escape double quotes and wrap the value in quotes if it contains a comma or newline
        return value.Contains(",") || value.Contains("\n")
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}
