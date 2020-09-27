using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Web.Services;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeRepositoryAsync _employee;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRazorRenderService _renderService;
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, IEmployeeRepositoryAsync employee, IUnitOfWork unitOfWork, IRazorRenderService renderService)
        {
            _logger = logger;
            _employee = employee;
            _unitOfWork = unitOfWork;
            _renderService = renderService;
        }
        public IEnumerable<Employee> Employees { get; set; }
        public void OnGet()
        {
        }
        public async Task<PartialViewResult> OnGetViewAllPartial()
        {
            Employees = await _employee.GetAllAsync();
            return new PartialViewResult
            {
                ViewName = "_ViewAll",
                ViewData = new ViewDataDictionary<IEnumerable<Employee>>(ViewData, Employees)
            };
        }
        public async Task<JsonResult> OnGetCreateOrEditAsync(int id = 0)
        {
            if (id == 0)
                return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", new Employee()) });
            else
            {
                var thisEmployee = await _employee.GetByIdAsync(id);
                return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", thisEmployee) });
            }
        }
        public async Task<JsonResult> OnPostCreateOrEditAsync(int id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _employee.AddAsync(employee);
                    await _unitOfWork.Commit();
                }
                else
                {
                    await _employee.UpdateAsync(employee);
                    await _unitOfWork.Commit();
                }
                Employees = await _employee.GetAllAsync();
                var html = await _renderService.ToStringAsync("_ViewAll", Employees);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var html = await _renderService.ToStringAsync("_CreateOrEdit", employee);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            var employee = await _employee.GetByIdAsync(id);
            await _employee.DeleteAsync(employee);
            await _unitOfWork.Commit();
            Employees = await _employee.GetAllAsync();
            var html = await _renderService.ToStringAsync("_ViewAll", Employees);
            return new JsonResult(new { isValid = true, html = html });
        }
    }
}
