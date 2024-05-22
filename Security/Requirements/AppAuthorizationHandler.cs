using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PBL3_Course.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirment=context.PendingRequirements;
            foreach(var requirment in pendingRequirment)
            {
                if(requirment is TeacherInCourse)
                {
                    if(checkCourse(context.Resource,(TeacherInCourse)requirment,context.User))
                    {
                        context.Succeed(requirment);
                    }
                }
            }
            return Task.CompletedTask;
        }
        public  bool checkCourse(object resource,TeacherInCourse requirment,ClaimsPrincipal user )
        {
            var course=resource as Course;
            if(course==null)
            {
                return false;
            }
            if(user.Claims.FirstOrDefault(c=>c.Type=="Id").Value==course.TeacherId.ToString()==false)
            {
                  return false;
            }

            return true;
        }

    }
}