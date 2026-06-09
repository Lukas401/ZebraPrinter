using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelManager.Domain.Interfaces;

public interface IPrintService
{
    Task EnviarZplAsync(string zpl);
}