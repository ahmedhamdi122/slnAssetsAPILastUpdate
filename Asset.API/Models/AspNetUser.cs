using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Calibrations = new HashSet<Calibration>();
            HospitalApplications = new HashSet<HospitalApplication>();
            RequestTrackings = new HashSet<RequestTracking>();
            Requests = new HashSet<Request>();
            SupplierExecludeAssets = new HashSet<SupplierExecludeAsset>();
            WorkOrderAssigns = new HashSet<WorkOrderAssign>();
            WorkOrderTrackings = new HashSet<WorkOrderTracking>();
            WorkOrders = new HashSet<WorkOrder>();
        }

        public string Id { get; set; }
        public int GovernorateId { get; set; }
        public int CityId { get; set; }
        public int OrganizationId { get; set; }
        public int SubOrganizationId { get; set; }
        public int HospitalId { get; set; }
        public string RoleId { get; set; }
        public int RoleCategoryId { get; set; }
        public int? SupplierId { get; set; }
        public int? CommetieeMemberId { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<Calibration> Calibrations { get; set; }
        public virtual ICollection<HospitalApplication> HospitalApplications { get; set; }
        public virtual ICollection<RequestTracking> RequestTrackings { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<SupplierExecludeAsset> SupplierExecludeAssets { get; set; }
        public virtual ICollection<WorkOrderAssign> WorkOrderAssigns { get; set; }
        public virtual ICollection<WorkOrderTracking> WorkOrderTrackings { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
