﻿using InventoryManagment.Web.Data;
using InventoryManagment.Web.Models;
using InventoryManagment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace InventoryManagment.Web.Controllers
{
	public class LaptopsController : Controller
	{
		private ApplicationDbContext _context;
		public LaptopsController(ApplicationDbContext context)
		{
			this._context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// returning list of laptops from database to Index view
			var laptops = await _context.Laptops.ToListAsync();

			return View(laptops);
		}
		[HttpGet]
		public IActionResult Add()
		{
			// creating ViewBag for user selection dropdown
			ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "Name");
			ViewBag.Producers = new SelectList(_context.Producers.ToList(), "Id", "Name");

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Add(AddLaptopViewModel viewModel)
		{
			// adding user provided data to object for database insertion
			var laptop = new Laptop
			{
				AssignedTo = viewModel.AssignedTo,
				Producer = viewModel.Producer,
				Model = viewModel.Model,
				InStock = viewModel.InStock,
				SerialNumber = viewModel.SerialNumber
			};

			await _context.AddAsync(laptop);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		[HttpGet]
		public async Task<IActionResult> Edit(int Id)
		{
			var laptop = await _context.Laptops.FindAsync(Id);

			ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "Name");
			ViewBag.Producers = new SelectList(_context.Producers.ToList(), "Id", "Name");

			return View(laptop);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(Laptop viewModel)
		{
			// searching for elements of database to update values
			var laptop = await _context.Laptops.FindAsync(viewModel.Id);

			// updating database values
			if (laptop is not null)
			{
				laptop.Id = viewModel.Id;
				laptop.AssignedTo = viewModel.AssignedTo;
				laptop.Producer = viewModel.Producer;
				laptop.Model = viewModel.Model;
				laptop.InStock = viewModel.InStock;
				laptop.SerialNumber = viewModel.SerialNumber;
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		[HttpPost]
		public async Task<IActionResult> Delete(Laptop viewModel)
		{
			// finding laptop to remove
			var laptop = await _context.Laptops.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == viewModel.Id);
			// removing laptop from database
			if (laptop is not null)
			{
				_context.Laptops.Remove(laptop);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Export()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			var laptops = await _context.Laptops.ToListAsync();

			byte[] bytes;
			using (var laptop = new ExcelPackage())
			{
				var sheet = laptop.Workbook.Worksheets.Add("Laptops");
				int i = 2;

				// creating headers for collumns
				sheet.Cells[1, 1].Value = "Id";
				sheet.Cells[1, 2].Value = "AssignedTo";
				sheet.Cells[1, 3].Value = "Producer";
				sheet.Cells[1, 4].Value = "Model";
				sheet.Cells[1, 5].Value = "InStock";
				sheet.Cells[1, 6].Value = "SerialNumber";

				// data insert to cells
				foreach (var item in laptops)
				{
					sheet.Cells[i, 1].Value = item.Id;
					sheet.Cells[i, 2].Value = item.AssignedTo;
					sheet.Cells[i, 3].Value = item.Producer;
					sheet.Cells[i, 4].Value = item.Model;
					sheet.Cells[i, 5].Value = item.InStock;
					sheet.Cells[i, 6].Value = item.SerialNumber;
					i++;
				}
				bytes = await laptop.GetAsByteArrayAsync();
			}
			// file download
			var file = new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			file.FileDownloadName = "Laptops" + DateOnly.FromDateTime(DateTime.Now).ToString() + ".xlsx";
			return file;
		}

	}
}
