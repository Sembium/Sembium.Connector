using Sembium.Connector.Data;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sembium.Connector.Entities;
using Microsoft.Win32;

namespace Sembium.Connector.Services
{
    public class EmployeesService :  IEmployeesService
    {
        private const int AutomatedAddEmployeeMovementsUserActivityCode = 5906;

        private readonly IDataConnection _dataConnection;
        private readonly IAuthorization _authorization;

        public EmployeesService(IDataConnection dataConnection, IAuthorization authorization)
        {
            _dataConnection = dataConnection;
            _authorization = authorization;
        }

        public void AddEmployeeMovement(int employeeNo, int inOut, DateTime movementDateTime)
        {
            _authorization.CheckUserActivity(AutomatedAddEmployeeMovementsUserActivityCode);

            TryAddEmployeeMovement(employeeNo, inOut, movementDateTime);
        }

        public void AddEmployeeMovements(IEnumerable<EmployeeMovement> employeeMovements)
        {
            _authorization.CheckUserActivity(AutomatedAddEmployeeMovementsUserActivityCode);

            foreach (var employeeMovement in employeeMovements)
            {
                TryAddEmployeeMovement(employeeMovement.EmployeeNo, employeeMovement.InOut, employeeMovement.MovementDateTime);
            }
        }

        private bool TryAddEmployeeMovement(int employeeNo, int inOut, DateTime movementDateTime)
        {
            try
            {
                DoAddEmployeeMovement(employeeNo, inOut, movementDateTime);
                return true;
            }
            catch (Exception e)
            {
                LogFailedEmployeeMovement(employeeNo, inOut, movementDateTime, e.Message);
                return false;
            }
        }

        private void DoAddEmployeeMovement(int employeeNo, int inOut, DateTime movementDateTime)
        {
            if ((inOut != -1) && (inOut != 1))
                throw new UserException("InOut must be -1 or 1");

            _dataConnection.ExecSql(
                "insert into EMP_MOVEMENTS_FOR_EDIT" + Environment.NewLine +
                "(" + Environment.NewLine +
                "  EMP_MOVEMENT_CODE," + Environment.NewLine +
                "  EMPLOYEE_CODE," + Environment.NewLine +
                "  IN_OUT," + Environment.NewLine +
                "  EMP_MOVEMENT_DATE," + Environment.NewLine +
                "  EMP_MOVEMENT_TIME," + Environment.NewLine +
                "  CREATE_EMPLOYEE_CODE," + Environment.NewLine +
                "  CREATE_DATE," + Environment.NewLine +
                "  CREATE_TIME" + Environment.NewLine +
                ")" + Environment.NewLine +
                "values" + Environment.NewLine +
                "(" + Environment.NewLine +
                "  seq_EMP_MOVEMENTS.NextVal," + Environment.NewLine +
                "  ( select" + Environment.NewLine +
                "      e.EMPLOYEE_CODE" + Environment.NewLine +
                "    from" + Environment.NewLine +
                "      COMPANIES c," + Environment.NewLine +
                "      EMPLOYEES e" + Environment.NewLine +
                "    where" + Environment.NewLine +
                "      (c.COMPANY_NO = :EMPLOYEE_NO) and" + Environment.NewLine +
                "      (c.COMPANY_CODE = e.COMPANY_CODE)" + Environment.NewLine +
                "  )," + Environment.NewLine +
                "  :IN_OUT," + Environment.NewLine +
                "  Trunc(:EMP_MOVEMENT_DATE_TIME)," + Environment.NewLine +
                "  TimeOf(:EMP_MOVEMENT_DATE_TIME)," + Environment.NewLine +
                "  LoginContext.UserCode," + Environment.NewLine +
                "  ContextDate," + Environment.NewLine +
                "  ContextTime" + Environment.NewLine +
                ")",
                new SqlDataParameter("EMPLOYEE_NO", employeeNo),
                new SqlDataParameter("IN_OUT", inOut),
                new SqlDataParameter("EMP_MOVEMENT_DATE_TIME", movementDateTime));
        }

        private void LogFailedEmployeeMovement(int employeeNo, int inOut, DateTime movementDateTime, string exceptionMessage)
        {
            _dataConnection.ExecSql(
                "insert into FAILED_EMP_MOVEMENTS" + Environment.NewLine +
                "(" + Environment.NewLine +
                "  FAILED_EMP_MOVEMENT_CODE," + Environment.NewLine +
                "  EMPLOYEE_NO," + Environment.NewLine +
                "  IN_OUT," + Environment.NewLine +
                "  EMP_MOVEMENT_DATE_TIME," + Environment.NewLine +
                "  CREATE_EMPLOYEE_CODE," + Environment.NewLine +
                "  CREATE_DATE," + Environment.NewLine +
                "  CREATE_TIME," + Environment.NewLine +
                "  EXCEPTION_MESSAGE" + Environment.NewLine +
                ")" + Environment.NewLine +
                "values" + Environment.NewLine +
                "(" + Environment.NewLine +
                "  seq_FAILED_EMP_MOVEMENTS.NextVal," + Environment.NewLine +
                "  :EMPLOYEE_NO," + Environment.NewLine +
                "  :IN_OUT," + Environment.NewLine +
                "  :EMP_MOVEMENT_DATE_TIME," + Environment.NewLine +
                "  LoginContext.UserCode," + Environment.NewLine +
                "  ContextDate," + Environment.NewLine +
                "  ContextTime," + Environment.NewLine +
                "  :EXCEPTION_MESSAGE" + Environment.NewLine +
                ")",
                new SqlDataParameter("EMPLOYEE_NO", employeeNo),
                new SqlDataParameter("IN_OUT", inOut),
                new SqlDataParameter("EMP_MOVEMENT_DATE_TIME", movementDateTime),
                new SqlDataParameter("EXCEPTION_MESSAGE", exceptionMessage));
        }
    }
}
