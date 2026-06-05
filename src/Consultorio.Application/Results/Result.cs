using System;
using System.Collections.Generic;

namespace Consultorio.Application.Results
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Ok(T data, string message = "Operacao realizada com sucesso")
            => new() { Success = true, Data = data, Message = message };

        public static Result<T> Fail(string message, params string[] errors)
            => new() { Success = false, Message = message, Errors = new List<string>(errors) };

        public static Result<T> ValidationFail(params string[] errors)
            => new() { Success = false, Message = "Validacao falhou", Errors = new List<string>(errors) };
    }

    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result Ok(string message = "Operacao realizada com sucesso")
            => new() { Success = true, Message = message };

        public static Result Fail(string message, params string[] errors)
            => new() { Success = false, Message = message, Errors = new List<string>(errors) };

        public static Result ValidationFail(params string[] errors)
            => new() { Success = false, Message = "Validacao falhou", Errors = new List<string>(errors) };
    }
}
