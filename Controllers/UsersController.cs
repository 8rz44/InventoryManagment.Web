﻿using InventoryManagment.Web.Data;
using InventoryManagment.Web.Models;
using InventoryManagment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagment.Web.Controllers
{
	public class UsersController : Controller
	{
		private ApplicationDbContext _context;
		public UsersController(ApplicationDbContext context)
		{
			this._context = context;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await _context.Users.ToListAsync();

			return View(users);
		}
		[HttpGet]
		public IActionResult Add()
		{
			ViewBag.Department = new SelectList(_context.Departments.ToList(), "Id", "Name");

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Add(AddUserViewModel viewModel)
		{
			var user = new User
			{
				Name = viewModel.Name,
				Email = viewModel.Email,
				Department = viewModel.Department,
				DateOfEmployment = viewModel.DateOfEmployment
			};

			await _context.AddAsync(user);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		[HttpGet]
		public async Task<IActionResult> Edit(Guid Id)
		{
			var user = await _context.Users.FindAsync(Id);

			ViewBag.Department = new SelectList(_context.Departments.ToList(), "Id", "Name");

			return View(user);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(User viewModel)
		{
			var user = await _context.Users.FindAsync(viewModel.Id);
			if (user is not null)
			{
				user.Name = viewModel.Name;
				user.Email = viewModel.Email;
				user.Department = viewModel.Department;
				user.DateOfEmployment = viewModel.DateOfEmployment;
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
		[HttpPost]
		public async Task<IActionResult> Delete(User viewModel)
		{
			try
			{
				var user = await _context.Users.AsNoTracking()
					.FirstOrDefaultAsync(x => x.Id == viewModel.Id);

				if (user is not null)
				{
					_context.Users.Remove(user);
					await _context.SaveChangesAsync();
				}
			}
			catch (Exception exception)
			{
				return View("DeleteUserError");
			}
			return RedirectToAction("Index");
		}

	}
}
