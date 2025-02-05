﻿using System;
using System.Collections.Generic;

namespace GrillPizzeriaBL.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Korisnik> Korisniks { get; } = new List<Korisnik>();
}
