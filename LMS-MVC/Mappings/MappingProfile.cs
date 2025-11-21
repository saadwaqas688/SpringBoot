using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;

namespace LMS_MVC.Mappings;

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
            .ForMember(dest => dest.Data, opt => opt.Ignore()); // Data is handled manually in service
        CreateMap<CreateLessonContentDto, LessonContent>()
            .ForMember(dest => dest.Data, opt => opt.Ignore()); // Data is handled manually in service
        CreateMap<UpdateLessonContentDto, LessonContent>()
            .ForMember(dest => dest.Data, opt => opt.Ignore()); // Data is handled manually in service

        // DiscussionPost mappings
        CreateMap<DiscussionPost, DiscussionPostDto>();
        CreateMap<CreateDiscussionPostDto, DiscussionPost>();
        CreateMap<UpdateDiscussionPostDto, DiscussionPost>();

        // LessonProgress mappings
        CreateMap<LessonProgress, LessonProgressDto>();
    }
}

