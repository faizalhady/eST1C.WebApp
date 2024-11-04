using System;

namespace eST1C.WebApp.DTO;

public class PCNameCountDTO
{
    public string? Workcell { get; set; }
    public int PcCount { get; set; }
    public DateTime? LastDateUsed { get; set; }

}
