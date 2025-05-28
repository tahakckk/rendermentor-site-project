using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class DrawParticipant
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }
}
