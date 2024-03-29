﻿using PurchaseBuddyLibrary.src.auth.app;

namespace PurchaseBuddy.src.purchases.app;

public class InvitationDto
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
	public Guid ListCreatorId { get; internal set; }
	public string ListCreatorName { get; internal set; }
	public DateTime CreatedAt { get; internal set; }
}
