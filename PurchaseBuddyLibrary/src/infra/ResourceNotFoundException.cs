﻿using System.Runtime.Serialization;

namespace PurchaseBuddy.src.infra;

[Serializable]
public class ResourceNotFoundException : Exception
{
	public ResourceNotFoundException()
	{
	}

	public ResourceNotFoundException(string? message) : base(message)
	{
	}

	public ResourceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}