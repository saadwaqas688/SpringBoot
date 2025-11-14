import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Home = () => {
  const { isAuthenticated, user } = useAuth();

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="max-w-4xl mx-auto px-4 py-16 text-center">
        <h1 className="text-5xl font-bold text-gray-900 mb-6">
          Course Management System
        </h1>
        <p className="text-xl text-gray-600 mb-12">
          Manage courses, discussions, and enrollments
        </p>

        {isAuthenticated ? (
          <div className="space-y-4">
            <p className="text-lg text-gray-700">
              Welcome back, <span className="font-semibold">{user?.email}</span>
              !
            </p>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mt-8">
              <Link
                to="/courses"
                className="bg-white hover:bg-blue-50 p-6 rounded-lg shadow-md transition-all transform hover:scale-105"
              >
                <div className="text-3xl mb-2">📚</div>
                <h3 className="font-bold text-gray-900">Courses</h3>
                <p className="text-sm text-gray-600">Manage courses</p>
              </Link>
              <Link
                to="/discussions"
                className="bg-white hover:bg-blue-50 p-6 rounded-lg shadow-md transition-all transform hover:scale-105"
              >
                <div className="text-3xl mb-2">💬</div>
                <h3 className="font-bold text-gray-900">Discussions</h3>
                <p className="text-sm text-gray-600">View discussions</p>
              </Link>
              {user?.role === "ADMIN" && (
                <>
                  <Link
                    to="/enrollments"
                    className="bg-white hover:bg-blue-50 p-6 rounded-lg shadow-md transition-all transform hover:scale-105"
                  >
                    <div className="text-3xl mb-2">👥</div>
                    <h3 className="font-bold text-gray-900">Enrollments</h3>
                    <p className="text-sm text-gray-600">Manage enrollments</p>
                  </Link>
                  <Link
                    to="/users"
                    className="bg-white hover:bg-blue-50 p-6 rounded-lg shadow-md transition-all transform hover:scale-105"
                  >
                    <div className="text-3xl mb-2">👤</div>
                    <h3 className="font-bold text-gray-900">Users</h3>
                    <p className="text-sm text-gray-600">Manage users</p>
                  </Link>
                </>
              )}
            </div>
          </div>
        ) : (
          <div className="space-y-4">
            <p className="text-lg text-gray-700 mb-8">
              Please sign in to access the system
            </p>
            <div className="flex justify-center space-x-4">
              <Link
                to="/login"
                className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-8 rounded-lg text-lg"
              >
                Sign In
              </Link>
              <Link
                to="/signup"
                className="bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-8 rounded-lg text-lg"
              >
                Sign Up
              </Link>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;
