//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MakeSale.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class C_Size
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_Size()
        {
            this.C_Product_Size = new HashSet<C_Product_Size>();
            this.C_RequestForQuotationLinePrice = new HashSet<C_RequestForQuotationLinePrice>();
        }
    
        public int C_Size_ID { get; set; }
        public string Size { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_Product_Size> C_Product_Size { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_RequestForQuotationLinePrice> C_RequestForQuotationLinePrice { get; set; }
    }
}
