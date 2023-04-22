﻿using System.CommandLine;
using ValidIp;

class Program
{
    static int Main(string[] args)
    {
        var isV4Option = new Option<string>(
            name: "--v4",
            description: "Test if <ip> is a valid IPv4"
        );

        var isV6Option = new Option<string>(
            name: "--v6",
            description: "Test if <ip> is a valid IPv6"
        );

        isV4Option.ArgumentHelpName = "ip";
        isV6Option.ArgumentHelpName = "ip";

        var rootCommand = new RootCommand(
            "Validate or generate random IP addresses.\n"
            + "Example usage: valid-ip --v4 8.8.8.8"
        );

        rootCommand.AddOption(isV4Option);
        rootCommand.AddOption(isV6Option);

        rootCommand.AddValidator(result =>
        {
            if (
                result.FindResultFor(isV4Option) is not null
                && result.FindResultFor(isV6Option) is not null
            )
            {
                result.ErrorMessage = "You cannot use options --v4 and --v6 together";
            }
        });

        rootCommand.SetHandler(
            (ipV4, ipV6) =>
            {
                if (ipV4 is not null)
                {
                    var result = Validate.IpV4(ipV4);
                    Console.WriteLine(result);
                    return;
                }

                if (ipV6 is not null)
                {
                    var result = Validate.IpV6(ipV6);
                    Console.WriteLine(result);
                    return;
                }
                else
                    rootCommand.Invoke("--help");
            },
            isV4Option,
            isV6Option
        );

        var generateV4Option = new Option<int>(
            name: "--v4",
            description: "Generate <amount> of IPv4 addresses"
        );

        var generateV6Option = new Option<int>(
            name: "--v6",
            description: "Generate <amount> of IPv6 addresses"
        );

        generateV4Option.ArgumentHelpName = "amount";
        generateV6Option.ArgumentHelpName = "amount";

        var generateCommand = new Command(
            name: "generate",
            description: "Generate random IP addresses.\n"
                + "Example usage: valid-ip generate --v4 8"
        );

        generateCommand.AddOption(generateV4Option);
        generateCommand.AddOption(generateV6Option);

        generateCommand.AddAlias("gen");

        rootCommand.AddCommand(generateCommand);

        generateCommand.AddValidator(result =>
        {
            if (result.FindResultFor(generateV4Option) is not null
                && result.FindResultFor(generateV6Option) is not null)
            {
                result.ErrorMessage = "You cannot use options --v4 and --v6 together";
            }
        });

        generateCommand.SetHandler(
            (ipV4Amount, ipV6Amount) =>
            {
                if (ipV4Amount is not 0)
                {
                    var result = Generate.IpV4(ipV4Amount);
                    Console.WriteLine(result);
                    return;
                }

                if (ipV6Amount is not 0)
                {
                    var result = Generate.IpV6(ipV6Amount);
                    Console.WriteLine(result);
                    return;
                }
                else
                    generateCommand.Invoke("--help");
            },
            generateV4Option,
            generateV6Option
        );

        return rootCommand.Invoke(args);
    }
}