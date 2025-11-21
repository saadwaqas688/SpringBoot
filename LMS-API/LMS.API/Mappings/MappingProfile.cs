using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using System.Text.Json;

namespace LMS.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();

        // Course mappings
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>();

        // Lesson mappings
        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<UpdateLessonDto, Lesson>();

        // LessonContent mappings
        CreateMap<LessonContent, LessonContentDto>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.Data) ? null : JsonSerializer.Deserialize<ContentDataDto>(src.Data)));
        CreateMap<CreateLessonContentDto, LessonContent>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => 
                src.Data != null ? JsonSerializer.Serialize(src.Data) : string.Empty));
        CreateMap<UpdateLessonContentDto, LessonContent>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => 
                src.Data != null ? JsonSerializer.Serialize(src.Data) : string.Empty));

        // DiscussionPost mappings
        CreateMap<DiscussionPost, DiscussionPostDto>();
        CreateMap<CreateDiscussionPostDto, DiscussionPost>();
        CreateMap<UpdateDiscussionPostDto, DiscussionPost>();

        // LessonProgress mappings
        CreateMap<LessonProgress, LessonProgressDto>();

        // Enrollment mappings
        CreateMap<UserCourse, EnrollmentDto>();
    }
}

