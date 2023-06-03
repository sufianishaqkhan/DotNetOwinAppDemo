using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinAppDemo.Controller.Services
{
    public class DemoService
    {
        public DemoService()
        {

        }

        public List<string> GetAllValues()
        {
            List<string> valuesList = new List<string>();

            valuesList.Add("service.value1");
            valuesList.Add("service.value2");
            valuesList.Add("service.value3");

            return valuesList;
        }
    }
}