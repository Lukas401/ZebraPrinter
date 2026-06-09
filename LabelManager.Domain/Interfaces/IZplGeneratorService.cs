using LabelManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelManager.Domain.Interfaces;

    public interface IZplGeneratorService
    {
        string GerarZpl(LabelData label);

        string GerarZplLote (IEnumerable<LabelData> labels);
    }
