﻿using App.Models.Inputs;
using System.CommandLine;
using System.CommandLine.Binding;

namespace App.Commands.Projects;

public class ProjectInputBinder : BinderBase<ProjectInput>
{
   private readonly Argument<int>? _id;
   private readonly Option<string?>? _name;
   private readonly Option<bool?>? _closed;
   private readonly Option<bool>? _manualMode;

   public ProjectInputBinder(Argument<int>? id = null, Option<string?>? name = null, Option<bool?>? closed = null, Option<bool>? manualMode = null)
   {
      _id = id;
      _name = name;
      _closed = closed;
      _manualMode = manualMode;
   }

   protected override ProjectInput GetBoundValue(BindingContext bindingContext)
   {
      return new ProjectInput
      {
         Id = _id is not null ? bindingContext.ParseResult.GetValueForArgument(_id) : 0,
         Name = _name is not null ? bindingContext.ParseResult.GetValueForOption(_name) : string.Empty,
         Closed = _closed is not null ? bindingContext.ParseResult.GetValueForOption(_closed) : false,
         ManualMode = _manualMode is not null && bindingContext.ParseResult.GetValueForOption(_manualMode),
      };
   }
}