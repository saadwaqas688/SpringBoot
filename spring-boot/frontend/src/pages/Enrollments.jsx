import { useState, useEffect } from "react";
import { enrollmentsAPI, coursesAPI, usersAPI } from "../services/api";
import { toast } from "react-toastify";
import ProtectedRoute from "../components/ProtectedRoute";
import { useAuth } from "../context/AuthContext";

const Enrollments = () => {
  const { isAdmin } = useAuth();
  const [courses, setCourses] = useState([]);
  const [allUsers, setAllUsers] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState("");
  const [enrolledUsers, setEnrolledUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showEnrollForm, setShowEnrollForm] = useState(false);
  const [selectedUserIds, setSelectedUserIds] = useState([]);

  useEffect(() => {
    fetchCourses();
    fetchAllUsers();
  }, []);

  useEffect(() => {
    if (selectedCourse) {
      fetchEnrolledUsers();
    }
  }, [selectedCourse]);

  const fetchCourses = async () => {
    try {
      setLoading(true);
      const data = await coursesAPI.getAll();
      setCourses(data);
    } catch (error) {
      toast.error("Failed to fetch courses");
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const fetchAllUsers = async () => {
    try {
      const data = await usersAPI.getAll();
      setAllUsers(data || []);
    } catch (error) {
      console.error("Failed to fetch users:", error);
      // Don't show error toast here, just log it
      // Users might not be available yet
    }
  };

  const fetchEnrolledUsers = async () => {
    if (!selectedCourse) return;
    try {
      const data = await enrollmentsAPI.getEnrolledUsers(selectedCourse);
      setEnrolledUsers(data);
    } catch (error) {
      if (error.response?.status === 403) {
        toast.error("Access denied. Admin role required.");
      } else {
        toast.error("Failed to fetch enrolled users");
      }
    }
  };

  const handleUserToggle = (userId) => {
    setSelectedUserIds((prev) => {
      if (prev.includes(userId)) {
        return prev.filter((id) => id !== userId);
      } else {
        return [...prev, userId];
      }
    });
  };

  const handleSelectAll = () => {
    if (selectedUserIds.length === allUsers.length) {
      setSelectedUserIds([]);
    } else {
      setSelectedUserIds(allUsers.map((user) => user.id));
    }
  };

  const handleEnroll = async (e) => {
    e.preventDefault();

    if (selectedUserIds.length === 0) {
      toast.error("Please select at least one user");
      return;
    }

    try {
      await enrollmentsAPI.enrollUsers(selectedCourse, selectedUserIds);
      toast.success("Users enrolled successfully!");
      setShowEnrollForm(false);
      setSelectedUserIds([]);
      fetchEnrolledUsers();
      fetchAllUsers(); // Refresh user list
    } catch (error) {
      if (error.response?.status === 403) {
        toast.error("Access denied. Admin role required.");
      } else {
        toast.error("Failed to enroll users");
      }
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
          <h1 className="text-3xl font-bold text-gray-900">Enrollments</h1>
          <button
            onClick={() => {
              setShowEnrollForm(true);
              fetchAllUsers(); // Refresh users when opening form
            }}
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            disabled={!selectedCourse}
          >
            + Enroll Users
          </button>
        </div>

        <div className="mb-6">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Select Course
          </label>
          <select
            value={selectedCourse}
            onChange={(e) => setSelectedCourse(e.target.value)}
            className="w-full md:w-1/3 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">Select a course</option>
            {courses.map((course) => (
              <option key={course.id} value={course.id}>
                {course.name}
              </option>
            ))}
          </select>
        </div>

        {showEnrollForm && (
          <div className="bg-white shadow-md rounded-lg p-6 mb-6">
            <h2 className="text-xl font-bold mb-4">Enroll Users to Course</h2>
            <form onSubmit={handleEnroll} className="space-y-4">
              <div>
                <div className="flex justify-between items-center mb-2">
                  <label className="block text-sm font-medium text-gray-700">
                    Select Users
                  </label>
                  <button
                    type="button"
                    onClick={handleSelectAll}
                    className="text-sm text-blue-600 hover:text-blue-800"
                  >
                    {selectedUserIds.length === allUsers.length
                      ? "Deselect All"
                      : "Select All"}
                  </button>
                </div>
                <div className="border border-gray-300 rounded-md max-h-64 overflow-y-auto">
                  {allUsers.length === 0 ? (
                    <div className="p-4 text-center text-gray-500">
                      <p>
                        No users available. Please ensure users exist in the
                        system.
                      </p>
                      <button
                        type="button"
                        onClick={fetchAllUsers}
                        className="mt-2 text-blue-600 hover:text-blue-800 text-sm"
                      >
                        Refresh Users
                      </button>
                    </div>
                  ) : (
                    <div className="p-2">
                      {allUsers.map((user) => (
                        <label
                          key={user.id}
                          className="flex items-center p-2 hover:bg-gray-50 rounded cursor-pointer"
                        >
                          <input
                            type="checkbox"
                            checked={selectedUserIds.includes(user.id)}
                            onChange={() => handleUserToggle(user.id)}
                            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                          />
                          <div className="ml-3 flex-1">
                            <div className="text-sm font-medium text-gray-900">
                              {user.firstName} {user.lastName}
                            </div>
                            <div className="text-xs text-gray-500">
                              {user.email} ({user.role})
                            </div>
                          </div>
                        </label>
                      ))}
                    </div>
                  )}
                </div>
                {selectedUserIds.length > 0 && (
                  <p className="mt-2 text-sm text-gray-600">
                    {selectedUserIds.length} user(s) selected
                  </p>
                )}
              </div>
              <div className="flex space-x-4">
                <button
                  type="submit"
                  className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                  disabled={selectedUserIds.length === 0}
                >
                  Enroll{" "}
                  {selectedUserIds.length > 0
                    ? `${selectedUserIds.length} `
                    : ""}
                  User{selectedUserIds.length !== 1 ? "s" : ""}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowEnrollForm(false);
                    setSelectedUserIds([]);
                  }}
                  className="bg-gray-500 hover:bg-gray-600 text-white font-bold py-2 px-4 rounded"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}

        {selectedCourse && (
          <div className="bg-white shadow-md rounded-lg p-6">
            <h2 className="text-xl font-bold mb-4">Enrolled Users</h2>
            {enrolledUsers.length === 0 ? (
              <p className="text-gray-500">
                No users enrolled in this course yet.
              </p>
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        User ID
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        First Name
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Last Name
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Email
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Enrolled Date
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {enrolledUsers.map((user) => (
                      <tr key={user.userId}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {user.userId}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {user.firstName}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {user.lastName}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {user.email}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {new Date(user.enrolledAt).toLocaleDateString()}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
};

export default Enrollments;
