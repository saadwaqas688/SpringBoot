import { Link, useLocation } from "react-router-dom";

const AdminSidebar = () => {
  const location = useLocation();

  const menuItems = [
    {
      name: "Courses",
      path: "/courses",
      icon: "📚",
    },
    {
      name: "Discussions",
      path: "/discussions",
      icon: "💬",
    },
    {
      name: "Enrollment",
      path: "/enrollments",
      icon: "👥",
    },
    {
      name: "Users",
      path: "/users",
      icon: "👤",
    },
  ];

  const isActive = (path) => {
    return location.pathname === path;
  };

  return (
    <div className="w-64 bg-gray-800 min-h-screen fixed left-0 top-16 z-40">
      <div className="p-4">
        <h2 className="text-white text-xl font-bold mb-6 px-4">Admin Panel</h2>
        <nav className="space-y-2">
          {menuItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center px-4 py-3 text-gray-300 rounded-lg transition-colors ${
                isActive(item.path)
                  ? "bg-blue-600 text-white"
                  : "hover:bg-gray-700 hover:text-white"
              }`}
            >
              <span className="mr-3 text-xl">{item.icon}</span>
              <span className="font-medium">{item.name}</span>
            </Link>
          ))}
        </nav>
      </div>
    </div>
  );
};

export default AdminSidebar;
