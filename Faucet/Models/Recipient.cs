using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

public partial class Recipient
{

    [JsonIgnore]
    [Key]

    public int id{ get; set; }
    public string address { get; set; }

    [JsonIgnore]
    public string ip_address {get; set; }

    public string transaction_id{get; set;}

    public bool is_sent{get; set;}

    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime created_at { get; set; }

}
