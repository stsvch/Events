using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // добавьте поля, если нужно: FirstName, LastName, …
    }
}
