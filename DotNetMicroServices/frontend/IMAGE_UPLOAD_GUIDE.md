# Image Upload Implementation Guide

## Overview

The image upload functionality allows users to upload course thumbnails through the frontend. Images are stored on the server and URLs are saved in the database.

## Architecture

### Backend Flow

1. **Upload Endpoint** (`/api/Upload/image` in CoursesService)

   - Accepts `IFormFile` via POST
   - Validates file type (jpg, jpeg, png, gif, webp)
   - Validates file size (max 5MB)
   - Saves to `wwwroot/uploads/courses/` directory
   - Returns URL: `http://localhost:5004/uploads/courses/{filename}`

2. **Static File Serving**

   - Files are served via `UseStaticFiles()` middleware
   - Accessible at: `http://localhost:5004/uploads/courses/{filename}`

3. **Gateway Proxy**
   - Gateway exposes `/api/Upload/image` endpoint
   - Forwards file uploads directly to CoursesService via HTTP (not RabbitMQ)
   - File uploads use direct HTTP calls because binary data doesn't serialize well in JSON messages

### Frontend Flow

1. **User selects image** in `CreateCourseModal`

   - File is stored in React state
   - Preview is shown using `FileReader` API

2. **On form submit**:

   - If new image selected: Upload image first using `useUploadImageMutation`
   - Get back image URL from upload response
   - Include thumbnail URL in course create/update payload

3. **Display images**:
   - Course cards check for `Thumbnail`, `thumbnail`, or `imageUrl` field
   - If URL exists, use as `backgroundImage` in `CourseImage` component
   - Otherwise, show first letter of course title

## File Structure

```
src/CoursesService/
├── Controllers/
│   └── UploadController.cs      # Handles image uploads
├── wwwroot/                     # Static files directory
│   └── uploads/
│       └── courses/            # Uploaded course images

frontend/src/
├── services/
│   └── upload-api.ts           # RTK Query API for uploads
├── components/
│   └── CreateCourseModal.tsx   # Image upload UI
└── app/(dashboard)/courses/
    └── page.tsx                # Displays course images
```

## API Endpoints

### Upload Image

```
POST /api/Upload/image
Content-Type: multipart/form-data
Body: file (IFormFile)

Response:
{
  "success": true,
  "message": "Image uploaded successfully",
  "data": "http://localhost:5004/uploads/courses/{guid}.jpg"
}
```

### Delete Image

```
DELETE /api/Upload/image/{fileName}

Response:
{
  "success": true,
  "message": "Image deleted successfully",
  "data": true
}
```

## Frontend Usage

```typescript
import { useUploadImageMutation } from "@/services/upload-api";

const [uploadImage, { isLoading }] = useUploadImageMutation();

// Upload image
const imageUrl = await uploadImage(file).unwrap();

// Use URL in course creation
await createCourse({
  Title: "Course Title",
  Description: "Description",
  Thumbnail: imageUrl,
});
```

## Image Display

Course cards automatically display images if `Thumbnail` field exists:

```tsx
<CourseImage
  sx={{
    backgroundImage: course.Thumbnail ? `url(${course.Thumbnail})` : undefined,
  }}
>
  {!course.Thumbnail && <>{course.Title?.charAt(0)}</>}
</CourseImage>
```

## Configuration

### Backend

- Upload directory: `wwwroot/uploads/courses/`
- Max file size: 5MB
- Allowed extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`

### Frontend

- Upload endpoint: `/api/Upload/image` (via Gateway)
- Base URL configured in `frontend/src/config.ts`

## Notes

- Images are stored locally in `wwwroot/uploads/courses/`
- For production, consider using cloud storage (Azure Blob, AWS S3, etc.)
- Gateway uses direct HTTP calls for file uploads (not RabbitMQ)
- Image URLs are stored in MongoDB as strings in the `Thumbnail` field




