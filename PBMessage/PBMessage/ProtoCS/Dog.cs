// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: dog.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Dog.Proto {

  /// <summary>Holder for reflection information generated from dog.proto</summary>
  public static partial class DogReflection {

    #region Descriptor
    /// <summary>File descriptor for dog.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DogReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cglkb2cucHJvdG8SCURvZy5Qcm90byIpCghMb2dpblJlcRIPCgd2ZXJzaW9u",
            "GAEgASgFEgwKBGFyZ3MYAiADKAkiLQoJTG9naW5SZXNwEg8KB3ZlcnNpb24Y",
            "ASABKAUSDwoHdXNlcl9pZBgCIAEoA0IiChZzdHVkaW8uZmlyc3QucHJvdG8u",
            "ZG9nQghEb2dQcm90b2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Dog.Proto.LoginReq), global::Dog.Proto.LoginReq.Parser, new[]{ "Version", "Args" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Dog.Proto.LoginResp), global::Dog.Proto.LoginResp.Parser, new[]{ "Version", "UserId" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class LoginReq : pb::IMessage<LoginReq> {
    private static readonly pb::MessageParser<LoginReq> _parser = new pb::MessageParser<LoginReq>(() => new LoginReq());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LoginReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Dog.Proto.DogReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq(LoginReq other) : this() {
      version_ = other.version_;
      args_ = other.args_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginReq Clone() {
      return new LoginReq(this);
    }

    /// <summary>Field number for the "version" field.</summary>
    public const int VersionFieldNumber = 1;
    private int version_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Version {
      get { return version_; }
      set {
        version_ = value;
      }
    }

    /// <summary>Field number for the "args" field.</summary>
    public const int ArgsFieldNumber = 2;
    private static readonly pb::FieldCodec<string> _repeated_args_codec
        = pb::FieldCodec.ForString(18);
    private readonly pbc::RepeatedField<string> args_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Args {
      get { return args_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as LoginReq);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(LoginReq other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Version != other.Version) return false;
      if(!args_.Equals(other.args_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Version != 0) hash ^= Version.GetHashCode();
      hash ^= args_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Version != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Version);
      }
      args_.WriteTo(output, _repeated_args_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Version != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Version);
      }
      size += args_.CalculateSize(_repeated_args_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(LoginReq other) {
      if (other == null) {
        return;
      }
      if (other.Version != 0) {
        Version = other.Version;
      }
      args_.Add(other.args_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Version = input.ReadInt32();
            break;
          }
          case 18: {
            args_.AddEntriesFrom(input, _repeated_args_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class LoginResp : pb::IMessage<LoginResp> {
    private static readonly pb::MessageParser<LoginResp> _parser = new pb::MessageParser<LoginResp>(() => new LoginResp());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LoginResp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Dog.Proto.DogReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginResp() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginResp(LoginResp other) : this() {
      version_ = other.version_;
      userId_ = other.userId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public LoginResp Clone() {
      return new LoginResp(this);
    }

    /// <summary>Field number for the "version" field.</summary>
    public const int VersionFieldNumber = 1;
    private int version_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Version {
      get { return version_; }
      set {
        version_ = value;
      }
    }

    /// <summary>Field number for the "user_id" field.</summary>
    public const int UserIdFieldNumber = 2;
    private long userId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long UserId {
      get { return userId_; }
      set {
        userId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as LoginResp);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(LoginResp other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Version != other.Version) return false;
      if (UserId != other.UserId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Version != 0) hash ^= Version.GetHashCode();
      if (UserId != 0L) hash ^= UserId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Version != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Version);
      }
      if (UserId != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(UserId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Version != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Version);
      }
      if (UserId != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(UserId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(LoginResp other) {
      if (other == null) {
        return;
      }
      if (other.Version != 0) {
        Version = other.Version;
      }
      if (other.UserId != 0L) {
        UserId = other.UserId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Version = input.ReadInt32();
            break;
          }
          case 16: {
            UserId = input.ReadInt64();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
