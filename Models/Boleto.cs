using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trabalho.Models;

namespace TrabalhoElvis2.Models
{
    public class Boleto
    {
        public int Id { get; set; }
<<<<<<< HEAD

        [Required]
        public int ContratoId { get; set; }

        // Navegação para o contrato (Imóvel + Morador vêm via Contrato)
        [ForeignKey(nameof(ContratoId))]
        public Contrato? Contrato { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime Vencimento { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Pagamento { get; set; }

        // "Pendente", "Pago", "Vencido", "Aguardando Confirmação"
        [Required, StringLength(30)]
        public string Status { get; set; } = "Pendente";

        // Guardar caminho da imagem gerada do QR Code (ex: /qrcodes/qrcode_123.png)
        public string? QrCodePix { get; set; }

        // Comprovante enviado pelo morador (imagem)
        public string? ComprovantePagamento { get; set; }
=======
        public int ContratoId { get; set; }
        public Contrato? Contrato { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }
        public DateTime Vencimento { get; set; }
        public DateTime? Pagamento { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pendente"; // Pendente, Pago, Vencido

        public string? QrCodePix { get; set; }              // Caminho do QR gerado
        public string? ComprovantePagamento { get; set; }   // Caminho do comprovante enviado
>>>>>>> 61ea956 (commit)
    }
}
