﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrafficWaveService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class bankasiaNSEntities : DbContext
    {
        public bankasiaNSEntities()
            : base("name=bankasiaNSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LOG_APP> LOG_APP { get; set; }
        public virtual DbSet<ow> ow { get; set; }
        public virtual DbSet<client_adress> client_adress { get; set; }
        public virtual DbSet<client_comp_details> client_comp_details { get; set; }
        public virtual DbSet<client_dl> client_dl { get; set; }
        public virtual DbSet<client_licence> client_licence { get; set; }
        public virtual DbSet<client_oborot> client_oborot { get; set; }
        public virtual DbSet<client_paspdata> client_paspdata { get; set; }
        public virtual DbSet<client_podpic> client_podpic { get; set; }
        public virtual DbSet<client_risk> client_risk { get; set; }
        public virtual DbSet<ClientOrg> ClientOrg { get; set; }
        public virtual DbSet<clientPhoto> clientPhoto { get; set; }
        public virtual DbSet<ClientProxy> ClientProxy { get; set; }
        public virtual DbSet<clients> clients { get; set; }
        public virtual DbSet<ENUMERATOR_SPR> ENUMERATOR_SPR { get; set; }
    
        public virtual ObjectResult<SearchReg_Result> SearchReg(string pFam, string pName, string pOtch, Nullable<byte> pStatus)
        {
            var pFamParameter = pFam != null ?
                new ObjectParameter("pFam", pFam) :
                new ObjectParameter("pFam", typeof(string));
    
            var pNameParameter = pName != null ?
                new ObjectParameter("pName", pName) :
                new ObjectParameter("pName", typeof(string));
    
            var pOtchParameter = pOtch != null ?
                new ObjectParameter("pOtch", pOtch) :
                new ObjectParameter("pOtch", typeof(string));
    
            var pStatusParameter = pStatus.HasValue ?
                new ObjectParameter("pStatus", pStatus) :
                new ObjectParameter("pStatus", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SearchReg_Result>("SearchReg", pFamParameter, pNameParameter, pOtchParameter, pStatusParameter);
        }
    }
}