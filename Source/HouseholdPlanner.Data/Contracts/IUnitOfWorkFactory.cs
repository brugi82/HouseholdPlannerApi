﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Contracts
{
    public interface IUnitOfWorkFactory
    {
		IUnitOfWork Create();
    }
}
