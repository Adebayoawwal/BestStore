using Crud.Models;
using Crud.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace Crud.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbCotnext context;
		private readonly IWebHostEnvironment environment;

		public ProductController(ApplicationDbCotnext context, IWebHostEnvironment environment)
        {
            this.context = context;
			this.environment = environment;
		}
        public IActionResult Index()
        {
            var product = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Create(ProductDto productDto)
        {
            if(productDto.ImageFile == null){
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid) { 
                return View(productDto);
            }


            string newfileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newfileName = Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newfileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            Product product = new Product() { 
                Name=productDto.Name,
                Brand=productDto.Brand,
                Category=productDto.Category,
                Price=productDto.Price,
                Description=productDto.Description,
                ImageFileName=newfileName,
                CreatedAt=DateTime.Now,
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Edit(int id) {
            Product product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index","Product");
            }
            Product productDto = new()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,

            };
            ViewData["ProductId"] =product. Id;
            ViewData["ImagefileName"]=product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            context.SaveChanges();
            return View(productDto);
        
            }
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Delete(int id)
        {
            Product product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            Product productDto = new()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,

            };
            ViewData["ProductId"] = product.Id;
            ViewData["ImagefileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            context.SaveChanges();
            return View(productDto);

        }
    }
    


}
