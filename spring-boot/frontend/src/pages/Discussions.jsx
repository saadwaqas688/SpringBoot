import { useState, useEffect } from "react";
import { discussionsAPI, coursesAPI, enrollmentsAPI } from "../services/api";
import { toast } from "react-toastify";
import ProtectedRoute from "../components/ProtectedRoute";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

const Discussions = () => {
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();
  const [discussions, setDiscussions] = useState([]);
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingDiscussion, setEditingDiscussion] = useState(null);
  const [formData, setFormData] = useState({
    courseId: "",
    title: "",
    description: "",
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [discussionsData, coursesData] = await Promise.all([
        discussionsAPI.getAll(),
        isAdmin() ? coursesAPI.getAll() : enrollmentsAPI.getMyEnrolledCourses(),
      ]);
      setDiscussions(discussionsData);
      setCourses(coursesData);
    } catch (error) {
      toast.error("Failed to fetch data");
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingDiscussion) {
        await discussionsAPI.update(editingDiscussion.id, formData);
        toast.success("Discussion updated successfully!");
      } else {
        await discussionsAPI.create(formData);
        toast.success("Discussion created successfully!");
      }
      setShowForm(false);
      setEditingDiscussion(null);
      setFormData({ courseId: "", title: "", description: "" });
      fetchData();
    } catch (error) {
      toast.error(
        editingDiscussion
          ? "Failed to update discussion"
          : "Failed to create discussion"
      );
    }
  };

  const handleEdit = (discussion) => {
    setEditingDiscussion(discussion);
    setFormData({
      courseId: discussion.courseId || "",
      title: discussion.title || "",
      description: discussion.description || "",
    });
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this discussion?")) {
      try {
        await discussionsAPI.delete(id);
        toast.success("Discussion deleted successfully!");
        fetchData();
      } catch (error) {
        const errorMessage =
          error.response?.data?.message ||
          error.message ||
          "Failed to delete discussion";
        toast.error(errorMessage);
      }
    }
  };

  const handleViewDetails = (discussionId) => {
    navigate(`/posts?discussionId=${discussionId}`);
  };

  const isOwnDiscussion = (discussion) => {
    return discussion.createdBy === user?.id;
  };

  const getCourseName = (courseId) => {
    const course = courses.find((c) => c.id === courseId);
    return course ? course.name : "Unknown Course";
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <ProtectedRoute>
      <div className="px-4 py-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Discussions</h1>
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            disabled={!isAdmin() && courses.length === 0}
          >
            + Add Discussion
          </button>
        </div>

        {showForm && (
          <div className="bg-white shadow-md rounded-lg p-6 mb-6">
            <h2 className="text-xl font-bold mb-4">
              {editingDiscussion ? "Edit Discussion" : "Create New Discussion"}
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Course
                </label>
                <select
                  name="courseId"
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                  value={formData.courseId}
                  onChange={handleChange}
                >
                  <option value="">Select a course</option>
                  {courses.map((course) => (
                    <option key={course.id} value={course.id}>
                      {course.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Title
                </label>
                <input
                  type="text"
                  name="title"
                  required
                  maxLength={200}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                  value={formData.title}
                  onChange={handleChange}
                  placeholder="Enter discussion title"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <textarea
                  name="description"
                  maxLength={2000}
                  rows={4}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                  value={formData.description}
                  onChange={handleChange}
                  placeholder="Enter discussion description"
                />
              </div>
              <div className="flex space-x-4">
                <button
                  type="submit"
                  className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                >
                  {editingDiscussion ? "Update" : "Create"}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowForm(false);
                    setEditingDiscussion(null);
                    setFormData({ courseId: "", title: "", description: "" });
                  }}
                  className="bg-gray-500 hover:bg-gray-600 text-white font-bold py-2 px-4 rounded"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {discussions.length === 0 ? (
            <div className="col-span-full text-center py-12">
              <p className="text-gray-500 text-lg">
                No discussions found. Create your first discussion!
              </p>
            </div>
          ) : (
            discussions.map((discussion) => (
              <div
                key={discussion.id}
                className="bg-white shadow-md rounded-lg p-6 hover:shadow-lg transition-shadow"
              >
                <div className="text-sm text-blue-600 font-semibold mb-2">
                  {getCourseName(discussion.courseId)}
                </div>
                <h3 className="text-xl font-bold text-gray-900 mb-2">
                  {discussion.title}
                </h3>
                <p className="text-gray-600 mb-4">
                  {discussion.description || "No description"}
                </p>
                <div className="text-sm text-gray-500 mb-4">
                  <p>
                    Created:{" "}
                    {new Date(discussion.createdAt).toLocaleDateString()}
                  </p>
                </div>
                <div className="flex space-x-2">
                  <button
                    onClick={() => handleViewDetails(discussion.id)}
                    className="flex-1 bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded text-sm"
                  >
                    View Details
                  </button>
                  {isOwnDiscussion(discussion) && (
                    <>
                      <button
                        onClick={() => handleEdit(discussion)}
                        className="flex-1 bg-yellow-500 hover:bg-yellow-600 text-white font-bold py-2 px-4 rounded text-sm"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(discussion.id)}
                        className="flex-1 bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded text-sm"
                      >
                        Delete
                      </button>
                    </>
                  )}
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </ProtectedRoute>
  );
};

export default Discussions;
