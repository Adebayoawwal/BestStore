using Crud.Models;
using Crud.Services;
using Microsoft.AspNetCore.Mvc;

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
            var product = context.Products.OrderByDescending(p => p.Id).OrderBy(x => x.Name).ToList();
            return View(product);
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Add(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }


            string newfileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newfileName = Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newfileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            Product product = new()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newfileName,
                CreatedAt = DateTime.Now,
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
        [HttpPost]
        public IActionResult Edit(int id)
        {
            Product product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            var productDto = new ProductDto()
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
        public IActionResult Edit(int id,ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImagefileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productDto);
            }

            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.Products.Update(product);
            context.SaveChanges();
            return RedirectToAction("Index,Product");
        }
     
        public IActionResult Delete(int id)
        {
            Product product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");

            }
            string ImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(ImageFullPath);
            context.Products.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Product");
        }
    }
};
