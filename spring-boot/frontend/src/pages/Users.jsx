import { useState, useEffect } from "react";
import { toast } from "react-toastify";
import ProtectedRoute from "../components/ProtectedRoute";
import { useAuth } from "../context/AuthContext";
import { usersAPI } from "../services/api";

const Users = () => {
  const { isAdmin } = useAuth();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (isAdmin()) {
      fetchUsers();
    }
  }, [isAdmin]);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const data = await usersAPI.getAll();
      setUsers(data || []);
    } catch (error) {
      console.error("Error fetching users:", error);

      // Handle different error cases
      if (error.response?.status === 404) {
        toast.info(
          "Users endpoint not found. Please ensure the backend is running and UserController is deployed."
        );
        setUsers([]);
      } else if (error.response?.status === 403) {
        toast.error("Access denied. Admin role required to view users.");
        setUsers([]);
      } else if (error.response?.status === 401) {
        toast.error("Unauthorized. Please login again.");
        setUsers([]);
      } else if (!error.response) {
        toast.error(
          "Cannot connect to backend. Please ensure the server is running on http://localhost:8080"
        );
        setUsers([]);
      } else {
        toast.error(
          `Failed to fetch users: ${
            error.response?.data?.message || error.message
          }`
        );
        setUsers([]);
      }
    } finally {
      setLoading(false);
    }
  };

  if (!isAdmin()) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-red-600 mb-4">
            Access Denied
          </h1>
          <p className="text-gray-600">
            You need admin privileges to access this page.
          </p>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <ProtectedRoute requireAdmin={true}>
      <div className="px-4 py-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Users</h1>
          <button
            onClick={fetchUsers}
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
          >
            Refresh
          </button>
        </div>

        {users.length === 0 ? (
          <div className="bg-white shadow-md rounded-lg p-6">
            <p className="text-gray-500 text-center">
              {loading ? "Loading users..." : "No users found."}
            </p>
            {!loading && (
              <div className="mt-4 text-sm text-gray-400 text-center">
                <p className="mb-2">
                  If you expected to see users, please check:
                </p>
                <ul className="list-disc list-inside space-y-1 text-left max-w-md mx-auto">
                  <li>Backend server is running on http://localhost:8080</li>
                  <li>
                    Backend has been restarted after UserController was added
                  </li>
                  <li>You are logged in as an ADMIN user</li>
                  <li>
                    Check browser console (F12) for detailed error messages
                  </li>
                </ul>
              </div>
            )}
          </div>
        ) : (
          <div className="bg-white shadow-md rounded-lg overflow-hidden">
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      ID
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Email
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      First Name
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Last Name
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Role
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Created At
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {users.map((user) => (
                    <tr key={user.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {user.id}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {user.email}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {user.firstName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {user.lastName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                            user.role === "ADMIN"
                              ? "bg-purple-100 text-purple-800"
                              : "bg-green-100 text-green-800"
                          }`}
                        >
                          {user.role}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {user.createdAt
                          ? new Date(user.createdAt).toLocaleDateString()
                          : "N/A"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
};

export default Users;
