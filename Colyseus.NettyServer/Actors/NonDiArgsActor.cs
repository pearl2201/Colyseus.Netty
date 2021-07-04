using System;
using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

public class NonDiArgsActor : ReceiveActor
{

    private readonly IServiceScope _scope;
    private string _arg1;
    private string _arg2;

    public NonDiArgsActor( IServiceProvider sp, string arg1, string arg2)
    {
        
        _scope = sp.CreateScope();
        _arg1 = arg1;
        _arg2 = arg2;

       
    }

    protected override void PreStart()
    {
       
    }

    protected override void PostStop()
    {
        _scope.Dispose();
    }
}