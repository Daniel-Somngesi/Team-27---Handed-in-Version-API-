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
    
    public partial class C_Label_Colour
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_Label_Colour()
        {
            this.C_RequestForQuotationLinePrice = new HashSet<C_RequestForQuotationLinePrice>();
        }
    
        public int C_Label_Colour_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_RequestForQuotationLinePrice> C_RequestForQuotationLinePrice { get; set; }
    }
}
