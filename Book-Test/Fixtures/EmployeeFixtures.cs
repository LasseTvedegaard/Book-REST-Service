namespace Book_Test.Fixtures {
    using Model;


    namespace Book_Test.Fixtures {
        public static class EmployeeFixtures {
            public static Employee GetAnEmployee() {
                Employee employeeToReturn = new Employee {
                    FirstName = "Tester1",
                    LastName = "Testersen1",
                    Address = "Testergade 1",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "99009901",
                    Email = "tt@mail1.com"
                };
                return employeeToReturn;
            }

            public static List<Employee> GetListOfEmployees() {
                List<Employee> listToReturn = new List<Employee> {
                new Employee {
                    FirstName = "Tester1",
                    LastName = "Testersen1",
                    Address = "Testergade 1",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "99009901",
                    Email = "tt@mail1.com"
                },
                new Employee {
                    FirstName = "Tester2",
                    LastName = "Testersen2",
                    Address = "Testergade 2",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "99009902",
                    Email = "tt@mail2.com"
                },
                new Employee {
                    FirstName = "Tester3",
                    LastName = "Testersen3",
                    Address = "Testergade 3",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "99009903",
                    Email = "tt@mail3.com"
                }
            };

                return listToReturn;
            }
        }
    }

}
