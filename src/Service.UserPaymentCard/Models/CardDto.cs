using System;

namespace Service.UserPaymentCard.Models
{
	public class CardDto
	{
		public Guid? CardId { get; set; }

		public string Name { get; set; }

		public string Number { get; set; }

		public string Holder { get; set; }

		public string Month { get; set; }

		public string Year { get; set; }

		public string Cvv { get; set; }

		public bool IsDefault { get; set; }
	}
}