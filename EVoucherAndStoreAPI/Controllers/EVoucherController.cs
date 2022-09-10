using EVoucherAndStoreAPI.Cache;
using EVoucherAndStoreAPI.DataAccess;
using EVoucherAndStoreAPI.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EVoucherController : ControllerBase
    {
        private readonly MySqlContext _context;
        public EVoucherController(MySqlContext context)
        {
            _context = context;
        }

        #region Vouchers

        [HttpGet("GetVouchers")]
        [Authorize(Roles = "Administrator")]
        [Cached(60)]
        public IActionResult GetVouchers()
        {
            try
            {                
                return Ok(_context.EVouchers.Include(x => x.Paymentmethods).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetVoucher/{number}")]
        [Authorize(Roles = "Administrator")]
        [Cached(60)]
        public IActionResult GetVoucherById(int number)
        {
            try
            {
                if (number == 0)
                    return BadRequest("Invalid request");

                return Ok(_context.EVouchers.Where(x => x.Id == number).Include(x => x.Paymentmethods).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("UpdateVoucher")]
        [Authorize(Roles = "Administrator")]
        public IActionResult SaveVoucher([FromBody] EVouchers request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                if (request.Id == 0)
                {
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    _context.EVouchers.AddAsync(request);
                    _context.SaveChanges();
                }
                else
                {
                    if (!_context.EVouchers.ToList().Any())
                        return BadRequest("No voucher found");

                    var payment = _context.EVouchers.FirstOrDefault(x => x.Id == request.Id);
                    if (payment is null)
                        return BadRequest("No voucher found");

                    payment.UpdatedOn = DateTime.Now;
                    payment.Title = request.Title;
                    payment.Description = request.Description;
                    payment.ExpiryDate = request.ExpiryDate;
                    payment.Image = request.Image;
                    payment.Amount = request.Amount;
                    payment.AvailablePaymentMethods = request.AvailablePaymentMethods;
                    payment.DiscountPaymentMethodId = request.DiscountPaymentMethodId;
                    payment.Quantity = request.Quantity;
                    payment.MaxVoucherLimit = request.MaxVoucherLimit;
                    payment.GiftPerUserLimit = request.GiftPerUserLimit;
                    payment.IsActive = request.IsActive;
                    _context.SaveChanges();
                }

                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        #endregion

        #region Vouchers

        [HttpGet("GetPaymentMethods")]
        [Authorize(Roles = "Administrator")]
        [Cached(60)]
        public IActionResult GetPaymentMethods()
        {
            try
            {
                return Ok(_context.PaymentMethods.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("UpdatePaymentMethod")]
        [Authorize]
        public IActionResult SavePaymentMethod([FromBody] PaymenMethods request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                if (request.PaymentMethodId == 0)
                {
                    _context.PaymentMethods.AddAsync(request);
                    _context.SaveChanges();
                }
                else
                {
                    if (!_context.PaymentMethods.ToList().Any())
                        return BadRequest("No Payment method found");

                    var payment = _context.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == request.PaymentMethodId);
                    if (payment is null)
                        return BadRequest("No Payment method found");

                    payment.Description = request.Description;
                    payment.PaymentMethodName = request.PaymentMethodName;
                    _context.SaveChanges();

                }

                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex?.InnerException?.Message?.ToString());
            }
        }

        #endregion

        #region Store

        [HttpGet("GetAllTransactions")]
        [Authorize(Roles = "Buyer")]
        [Cached(60)]
        public IActionResult GetAllTransactions()
        {
            try
            {
                return Ok(_context.Transactions.Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetTransactions/{number}")]
        [Authorize(Roles = "Buyer")]
        [Cached(60)]
        public IActionResult GetTransactionsByPhoneNumber(string number)
        {
            try
            {
                if (string.IsNullOrEmpty(number))
                    return BadRequest("Invalid request");

                return Ok(_context.Transactions.Where(x => x.PhoneNumber == number).Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetTransaction/{number}")]
        [Authorize(Roles = "Buyer")]
        [Cached(60)]
        public IActionResult GetTransactionById(int number)
        {
            try
            {
                if (number == 0)
                    return BadRequest("Invalid request");

                return Ok(_context.Transactions.Where(x => x.Id == number).Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("BuyVoucher")]
        [Authorize(Roles = "Buyer")]
        public IActionResult BuyVoucher([FromBody] TransactionModel request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                if (request.VoucherId == 0)
                    return BadRequest("Please select a voucher");

                var voucher = _context.EVouchers?.FirstOrDefault(x => x.Id == request.VoucherId);
                if (voucher is null)
                    return BadRequest("Invalid Voucher");

                if (request.Id == 0)
                {
                    var validToCreate = false;
                    var transactions = _context.Transactions;
                    if (transactions is null)
                    {
                        if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit > 0))
                            validToCreate = true;

                        if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit > 0))
                            validToCreate = true;
                    }
                    else
                    {
                        var existingTransactions = transactions.Where(x => x.VoucherId == request.VoucherId);
                        var numberOfVouchers = existingTransactions.Count();
                        if ( voucher.Quantity > numberOfVouchers)
                        {
                            var currentUserVouchers = existingTransactions.Where(x => x.PhoneNumber == request.PhoneNumber);
                            if (!currentUserVouchers.Any())
                            {
                                if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit > 0))
                                    validToCreate = true;

                                if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit > 0))
                                    validToCreate = true;
                            }
                            else if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit > currentUserVouchers.Count()))
                            {
                                validToCreate = true;
                            }
                            else if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit > currentUserVouchers.Count()))
                            {
                                validToCreate = true;
                            }
                        }
                    }

                    if (!validToCreate)
                        return StatusCode(500, "Limitation of buying this voucher is reached. Please try another voucher.");

                    request.PromoCode = GeneratePromoCode();
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    _context.Transactions.AddAsync(request);
                    _context.SaveChanges();
                }
                else
                {
                    if (!_context.EVouchers.ToList().Any())
                        return BadRequest("No Transaction found");

                    var result = _context.Transactions.FirstOrDefault(x => x.Id == request.Id);
                    if (result is null)
                        return BadRequest("No Transaction found");

                    result.UpdatedOn = DateTime.Now;
                    result.IsActive = request.IsActive;
                    _context.SaveChanges();
                }

                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        private string GeneratePromoCode()
        {
            return $"{GenerateSixDigit()}{GenerateFiveAlphabets()}";
        }

        private string GenerateSixDigit()
        {
            var chars = "0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        private string GenerateFiveAlphabets()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        #endregion
    }
}
