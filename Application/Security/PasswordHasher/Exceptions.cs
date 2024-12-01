using Domain.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Security;

public abstract class PasswordHasherException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{}

public class PasswordFormatIncorrectException()
    : PasswordHasherException($"Incorrect password format.");

public class PasswordIncorrectException()
    : PasswordHasherException($"Incorrect password.");
