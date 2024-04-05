﻿namespace Domain.Entities;
using System;
using System.Collections.Generic;


public partial class User
{
    public Guid UserId { get; set; }

    public string SubId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Name { get; set; }

    public string? Lastname { get; set; }

    public string? Username { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
