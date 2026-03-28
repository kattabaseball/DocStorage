using AutoMapper;
using TourDocs.Core.DTOs.Audit;
using TourDocs.Core.DTOs.Cases;
using TourDocs.Core.DTOs.Checklists;
using TourDocs.Core.DTOs.Dashboard;
using TourDocs.Core.DTOs.DocumentRequests;
using TourDocs.Core.DTOs.Documents;
using TourDocs.Core.DTOs.HardCopy;
using TourDocs.Core.DTOs.Members;
using TourDocs.Core.DTOs.Notifications;
using TourDocs.Core.DTOs.Organizations;
using TourDocs.Core.DTOs.Subscriptions;
using TourDocs.Domain.Entities;

namespace TourDocs.Core.Mappings;

/// <summary>
/// AutoMapper profile defining all entity-to-DTO mappings.
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Organization mappings
        CreateMap<Organization, OrganizationResponse>();
        CreateMap<Organization, OrganizationDetailResponse>()
            .ForMember(d => d.MemberCount, opt => opt.MapFrom(s => s.Members.Count))
            .ForMember(d => d.UserCount, opt => opt.MapFrom(s => s.OrganizationMembers.Count))
            .ForMember(d => d.CaseCount, opt => opt.MapFrom(s => s.Cases.Count));
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<UpdateOrganizationRequest, Organization>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<OrganizationMember, OrganizationMemberResponse>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email ?? string.Empty));

        // Member mappings
        CreateMap<Member, MemberResponse>()
            .ForMember(d => d.DocumentCount, opt => opt.MapFrom(s => s.Documents.Count));
        CreateMap<CreateMemberRequest, Member>();
        CreateMap<UpdateMemberRequest, Member>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Document mappings
        CreateMap<Document, DocumentResponse>()
            .ForMember(d => d.MemberName,
                opt => opt.MapFrom(s => $"{s.Member.LegalFirstName} {s.Member.LegalLastName}"))
            .ForMember(d => d.VersionCount, opt => opt.MapFrom(s => s.Versions.Count))
            .ForMember(d => d.CurrentVersionNumber,
                opt => opt.MapFrom(s => s.CurrentVersion != null ? s.CurrentVersion.VersionNumber : 0));

        // Case mappings
        CreateMap<Case, CaseResponse>()
            .ForMember(d => d.MemberCount, opt => opt.MapFrom(s => s.CaseMembers.Count));
        CreateMap<CreateCaseRequest, Case>();

        // Checklist mappings
        CreateMap<Checklist, ChecklistResponse>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));
        CreateMap<ChecklistItem, ChecklistItemResponse>();
        CreateMap<CreateChecklistRequest, Checklist>();
        CreateMap<CreateChecklistItemRequest, ChecklistItem>();

        // Hard copy mappings
        CreateMap<HardCopyRequest, HardCopyRequestResponse>()
            .ForMember(d => d.DocumentTitle, opt => opt.MapFrom(s => s.Document.Title))
            .ForMember(d => d.CaseName, opt => opt.MapFrom(s => s.Case.Name))
            .ForMember(d => d.RequestedByName, opt => opt.MapFrom(s => s.RequestedByUser.FullName))
            .ForMember(d => d.Handovers, opt => opt.MapFrom(s => s.Handovers));
        CreateMap<HardCopyHandover, HardCopyHandoverResponse>()
            .ForMember(d => d.FromUserName, opt => opt.MapFrom(s => s.FromUser.FullName))
            .ForMember(d => d.ToUserName, opt => opt.MapFrom(s => s.ToUser.FullName));
        CreateMap<CreateHardCopyRequestDto, HardCopyRequest>();

        // Document request mappings
        CreateMap<DocumentRequest, DocumentRequestResponse>()
            .ForMember(d => d.MemberName,
                opt => opt.MapFrom(s => $"{s.Member.LegalFirstName} {s.Member.LegalLastName}"))
            .ForMember(d => d.RequestedByName, opt => opt.MapFrom(s => s.RequestedByUser.FullName))
            .ForMember(d => d.CaseName, opt => opt.MapFrom(s => s.Case != null ? s.Case.Name : null));
        CreateMap<CreateDocumentRequestDto, DocumentRequest>();

        // Notification mappings
        CreateMap<Notification, NotificationResponse>();

        // Subscription mappings
        CreateMap<Subscription, SubscriptionResponse>();

        // Audit log mappings
        CreateMap<AuditLog, AuditLogResponse>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : null));
        CreateMap<AuditLog, RecentActivityResponse>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : null));
    }
}
