﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagment.Web.Models.Entities
{
	public class HardwareModel
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		[ForeignKey("Id")]
		public int Producer {  get; set; }
		public string Category { get; set; }
	}
}
