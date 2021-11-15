namespace SoftUni
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using SoftUni.Data;
    using SoftUni.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var db = new SoftUniContext();

            string result = RemoveTown(db);

            Console.WriteLine(result);
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            var AddressesToDelete = context.Addresses
                .Where(a => a.TownId == townToRemove.TownId);

            var employeesToDelete = context.Employees
                .Where(e => AddressesToDelete.Any(a => a.AddressId == e.AddressId));

            var countOfAddressRemoved = AddressesToDelete.Count();

            foreach (var employee in employeesToDelete)
            {
                employee.AddressId = null;
            }

            foreach (var address in AddressesToDelete)
            {
                context.Addresses.Remove(address);
            }

            context.Towns.Remove(townToRemove);

            context.SaveChanges();

            return $"{countOfAddressRemoved} addresses in Seattle were deleted";
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToDelete = context.Projects
                .FirstOrDefault(p => p.ProjectId == 2);

            var employeeProject = context.EmployeesProjects
                .Where(p => p.EmployeeId == 2)
                .ToList();


            foreach (var project in employeeProject)
            {
                context.EmployeesProjects.Remove(project);
            }

            context.Projects.Remove(projectToDelete);

            context.SaveChanges();

            var result = new StringBuilder();

            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            foreach (var project in projects)
            {
                result.AppendLine($"{project}");
            }

            return result.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var allEmployeesWithSa = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                })
                .Where(x => x.FirstName.StartsWith("Sa"))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            var output = string.Join(Environment.NewLine, allEmployeesWithSa
                .Select(x => $"{x.FirstName} {x.LastName} - {x.JobTitle} - (${x.Salary:f2})"));

            return output;
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var increaseSalariesEmployees = context.Employees
                .Include(x => x.Department)
                .Where(x => x.Department.Name == "Engineering" || x.Department.Name == "Tool Design"
                || x.Department.Name == "Marketing" || x.Department.Name == "Information Services")
                .ToArray();

            foreach (var emp in increaseSalariesEmployees)
            {
                emp.Salary *= 1.12m;
            }

            context.SaveChanges();

            var orderedData = increaseSalariesEmployees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            var output = string.Join(Environment.NewLine, orderedData
                .Select(x => $"{x.FirstName} {x.LastName} (${x.Salary:F2})"));
            return output;
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var lastTenStartedProjects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .ToArray();

            var data = lastTenStartedProjects.Select(x => new
            {
                x.Name,
                x.Description,
                x.StartDate
            }).OrderBy(x => x.Name);

            var sb = new StringBuilder();

            foreach (var project in data)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departmants = context.Departments
                .Include(x => x.Employees)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees.Select(e => new
                    {
                        EmployeeFirstName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        e.JobTitle
                    })

                })
                .Where(x => x.Employees.Count() > 5)
                .OrderBy(x => x.Employees.Count())
                .ThenBy(x => x.Name)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var d in departmants)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName}  {d.ManagerLastName} ");

                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var empProjects = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    EmployeeProjects = x.EmployeesProjects.Select(x => new
                    {
                        x.Project.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()
                })
                .Where(x => x.EmployeeId == 147)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var emp in empProjects)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");

                foreach (var p in emp.EmployeeProjects)
                {
                    sb.AppendLine($"{p.Name}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var allAdresses = context.Addresses
                .Include(x => x.Town)
                .Select(x => new
                {
                    x.AddressText,
                    TownName = x.Town.Name,
                    CountEmp = x.Employees.Count(),
                })
                .OrderByDescending(x => x.CountEmp)
                .ThenBy(x => x.TownName)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToArray();

            var output = string.Join(Environment.NewLine, allAdresses
                .Select(x => $"{x.AddressText}, {x.TownName} - {x.CountEmp} employees"));

            return output;
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var allEmployeesProjects = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001
                            && p.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    EmployeeProjects = x.EmployeesProjects.Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate,
                        EndDate = ep.Project.EndDate
                    })
                })
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var ep in allEmployeesProjects)
            {
                sb.AppendLine($"{ep.FirstName} {ep.LastName} - Manager: {ep.ManagerFirstName} {ep.ManagerLastName}");
                foreach (var p in ep.EmployeeProjects)
                {
                    string ended = p.EndDate.HasValue
                        ? p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                        : "not finished";
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {ended}");
                }

            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            string text = "Vitoshka 15";
            int townId = 4;

            var newAddress = new Address() { AddressText = text, TownId = townId };

            context.Addresses.Add(newAddress);

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            nakov.Address = newAddress;

            context.SaveChanges();

            var allEmployees = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => new
                {
                    AddressText = x.Address.AddressText
                })
                .ToArray();

            return string.Join(Environment.NewLine, allEmployees
                .Select(x => $"{x.AddressText}"));
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(x => x.Department)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    DepartmentName = x.Department.Name,
                    x.Salary
                }).Where(x => x.DepartmentName == "Research and Development")
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .ToList();

            var output = string.Join(Environment.NewLine, employees
                .Select(x => $"{x.FirstName} {x.LastName} from {x.DepartmentName} - ${x.Salary:F2}"));

            return output;
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employeesSalary = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary
                }).Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName);

            var output = string.Join(Environment.NewLine, employeesSalary
                .Select(x => $"{x.FirstName} - {x.Salary:f2}"));

            return output;
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            /*03. Employees Full Information
             Now we can use SoftUniContext to extract data from our database.
             Your first task is to extract all employees and return their first, last,
             and middle name, their job title, and salary, rounded to 2 symbols after
             the decimal separator, all of those separated with a space.
             Order them by employee id.
            */

            var db = context.Employees.Select(x => new
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                JobTitle = x.JobTitle,
                Salary = x.Salary,
                EmployeeId = x.EmployeeId
            }).OrderBy(x => x.EmployeeId);

            var output = string.Join(Environment.NewLine, db
                .Select(x => $"{x.FirstName} {x.LastName} {x.MiddleName} {x.JobTitle} {x.Salary:F2}"));

            return output;
        }
    }
}

