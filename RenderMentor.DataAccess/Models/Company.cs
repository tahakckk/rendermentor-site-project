﻿using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string StreetAddress { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string PhoneNumber { get; set; }

    public bool? IsAuthorizedCompany { get; set; }
}
