
import React, { useState, useEffect } from 'react';

export default function App() {
    const [employees, setEmployees] = useState([]);
    const [employeeIdToDelete, setEmployeeIdToDelete] = useState(null);
    const [editingEmployee, setEditingEmployee] = useState(null);
    const [updatedName, setUpdatedName] = useState('');
    const [updatedValue, setUpdatedValue] = useState('');
    const [newEmployee, setNewEmployee] = useState({ name: '' });
    const [sumResults, setSumResults] = useState([]);

    useEffect(() => {
        fetchEmployees();
    }, []);

    // Function to fetch employees
    const fetchEmployees = async () => {
        try {
            const response = await fetch('http://localhost:41478/Employees/GetEmployeeList');
            const data = await response.json();
            setEmployees(data);
            fetchEmployees();
            fetchSumOfValues();
        } catch (error) {
            console.error("Error fetching employees:", error);
        }
    };

    useEffect(() => {
        fetchSumOfValues();
    }, []);

    const fetchSumOfValues = async () => {
        try {
            const response = await fetch('http://localhost:41478/Employees/GetSumOfValues');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            setSumResults(data);
        } catch (error) {
            console.error("Error fetching sum of values:", error);
        }
    };


    useEffect(() => {
        if (employeeIdToDelete !== null) {
            const confirmed = window.confirm("Are you sure you want to delete this?");
            if (confirmed) {
                deleteEmployee(employeeIdToDelete);
            }
            setEmployeeIdToDelete(null); // Clear the ID after confirming
        }
    }, [employeeIdToDelete]);

    // Function to delete employee
    const deleteEmployee = async (id) => {
        try {
            const response = await fetch(`http://localhost:41478/Employees/DeleteEmployee/${id}`, {
                method: 'DELETE',
            });
           
            if (response.ok) {
                console.log("Employee deleted successfully");
                fetchEmployees();
            } else {
                console.error("Failed to delete employee:", response.statusText);
            }
        } catch (error) {
            console.error("Error deleting employee:", error);
        }
    };

    const editEmployee = (employee) => {
        setEditingEmployee(employee);
        setUpdatedName(employee.name);
        setUpdatedValue(employee.value); 
    };

    const updateEmployee = async () => {
        if (editingEmployee) {
            try {

                if (updatedName.trim() === '') {
                    alert("Please enter name.");
                    return;
                }

                const response = await fetch(`http://localhost:41478/Employees/UpdateEmployee/${editingEmployee.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        name: updatedName,
                    }),
                });

            

                if (response.ok) {
                    const updatedEmployees = employees.map((emp) =>
                        emp.id === editingEmployee.id ? { ...emp, name: updatedName, value: updatedValue } : emp
                    );
                    setEmployees(updatedEmployees);
                    setEditingEmployee(null);
                    setUpdatedName('');
                    setUpdatedValue('');
                    fetchEmployees();
                    fetchSumOfValues();
                } else {
                    console.error('Failed to update employee:', response.statusText);
                }
            } catch (error) {
                console.error('Error updating employee:', error);
            }
        }
    };

    // Function to add a new employee
    const addEmployee = async () => {
        if (newEmployee.name) {
            try {
                const response = await fetch('http://localhost:41478/Employees/AddEmployee', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(newEmployee),
                });
               
                if (response.ok) {
                    const addedEmployee = await response.json();
                    setEmployees([...employees, addedEmployee]);
                    setNewEmployee({ name: '' });
                } else {
                    console.error('Failed to add employee:', response.statusText);
                }
            } catch (error) {
                console.error('Error adding employee:', error);
            }
        } else {
            alert("Please enter name");
        }
    };

    useEffect(() => {
        fetchEmployees();
        fetchSumOfValues();
    }, []);

    const tableStyle = {
        borderCollapse: 'collapse',    
    };
    const cellStyle = {
        border: '1px solid #000',
        padding: '8px',
        textAlign: 'left',
    };

    const headerStyle = {
        ...cellStyle,
        backgroundColor: '#f2f2f2',
    };
    return (

        <div>
            <div>
                <h2>{editingEmployee ? "Edit Employee" : "Add New Employee"}</h2>
                <input
                    type="text"
                    placeholder="Name"
                    value={editingEmployee ? updatedName : newEmployee.name}
                    onChange={(e) => {
                        if (editingEmployee) {
                            setUpdatedName(e.target.value);
                        } else {
                            setNewEmployee({ ...newEmployee, name: e.target.value });
                        }
                    }}
                />
                
                <button onClick={editingEmployee ? updateEmployee : addEmployee}>
                    {editingEmployee ? 'Update Employee' : 'Add Employee'}
                </button>
                {editingEmployee && (
                    <button onClick={() => setEditingEmployee(null)}>Cancel</button>
                )}
            </div>

            <h1>Employee List</h1>
            <table style={tableStyle}>
                <thead>
                    <tr>
                        <th style={headerStyle}>Name</th>
                        <th style={headerStyle}>Value</th>
                        <th style={headerStyle}></th>
                    </tr>
                </thead>
                <tbody>
                    {employees.map(employee => (
                        <tr>
                            <td style={cellStyle}>{employee.name}</td>
                            <td style={cellStyle}>{employee.value}</td>
                            <td style={cellStyle}><button onClick={() => editEmployee(employee)}>Edit</button>  &nbsp;
                                <button onClick={() => setEmployeeIdToDelete(employee.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <h3>Sum of A, B or C Begins Name </h3>
            <ul>
                {sumResults.length > 0 ? (
                    sumResults.map((sum, index) => (
                        <li key={index}>{sum}</li>
                    ))
                ) : (
                    <li>No sums available or less than 11171</li>
                )}
            </ul>

        </div>
    );
}
