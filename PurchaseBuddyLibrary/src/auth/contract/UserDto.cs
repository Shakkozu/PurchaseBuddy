﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PurchaseBuddyLibrary.src.auth.contract;
public class UserDto
{
	[Required]
	[MinLength(4)]
	public string Login { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[MinLength(6)]
	[PasswordPropertyText]
	public string Password { get; set; }
}
