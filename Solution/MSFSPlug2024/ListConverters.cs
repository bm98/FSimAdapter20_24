using System;

using FS = Microsoft.FlightSimulator.SimConnect;
using CX = MSFSAdapter20_24;

namespace MSFSPlug2024
{
  /// <summary>
  /// Explicite Type mapping of lists and list items due to namespace issue 
  ///  cannot cast or otherwise map one instance to another 
  /// 
  /// takes care of the following lists:
  ///   SIMCONNECT_RECV_FACILITY_MINIMAL_LIST
  ///   SIMCONNECT_RECV_AIRPORT_LIST
  ///   SIMCONNECT_RECV_WAYPOINT_LIST
  ///   SIMCONNECT_RECV_NDB_LIST
  ///   SIMCONNECT_RECV_VOR_LIST
  ///   SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS
  ///   SIMCONNECT_RECV_CONTROLLERS_LIST
  ///   SIMCONNECT_RECV_JETWAY_DATA
  ///   SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST
  /// </summary>
  internal static class ListConverters
  {
    #region Generic struct converters

    public static CX.SIMCONNECT_DATA_XYZ GetFrom( FS.SIMCONNECT_DATA_XYZ orig )
    {
      return new CX.SIMCONNECT_DATA_XYZ( ) { x = orig.x, y = orig.y, z = orig.z };
    }
    public static CX.SIMCONNECT_DATA_LATLONALT GetFrom( FS.SIMCONNECT_DATA_LATLONALT orig )
    {
      return new CX.SIMCONNECT_DATA_LATLONALT( ) { Latitude = orig.Latitude, Longitude = orig.Longitude, Altitude = orig.Altitude };
    }
    public static CX.SIMCONNECT_DATA_PBH GetFrom( FS.SIMCONNECT_DATA_PBH orig )
    {
      return new CX.SIMCONNECT_DATA_PBH( ) { Pitch = orig.Pitch, Bank = orig.Bank, Heading = orig.Heading };
    }
    public static CX.SIMCONNECT_ICAO GetFrom( FS.SIMCONNECT_ICAO orig )
    {
      return new CX.SIMCONNECT_ICAO( ) {
        Airport = orig.Airport,
        Ident = orig.Ident,
        Region = orig.Region,
        Type = orig.Type
      };
    }

    #endregion

    #region SIMCONNECT_RECV_FACILITY_MINIMAL_LIST

    // used by SIMCONNECT_RECV_FACILITY_MINIMAL_LIST
    public static CX.SIMCONNECT_FACILITY_MINIMAL GetFrom( FS.SIMCONNECT_FACILITY_MINIMAL orig )
    {
      return new CX.SIMCONNECT_FACILITY_MINIMAL( ) {
        icao = GetFrom( orig.icao ), // needs Ident adjustment
        lla = new CX.SIMCONNECT_DATA_LATLONALT( ) { Latitude = orig.lla.Latitude, Longitude = orig.lla.Longitude, Altitude = orig.lla.Altitude },
      };
    }
    public static CX.SIMCONNECT_RECV_FACILITY_MINIMAL_LIST GetFrom( FS.SIMCONNECT_RECV_FACILITY_MINIMAL_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_FACILITY_MINIMAL_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_FACILITY_MINIMAL)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_AIRPORT_LIST

    // used by SIMCONNECT_RECV_AIRPORT_LIST
    public static CX.SIMCONNECT_DATA_FACILITY_AIRPORT GetFrom( FS.SIMCONNECT_DATA_FACILITY_AIRPORT orig )
    {
      return new CX.SIMCONNECT_DATA_FACILITY_AIRPORT( ) {
        Ident = orig.Ident,
        Region = orig.Region,
        Altitude = orig.Altitude,
        Latitude = orig.Latitude,
        Longitude = orig.Longitude,
      };
    }
    public static CX.SIMCONNECT_RECV_AIRPORT_LIST GetFrom( FS.SIMCONNECT_RECV_AIRPORT_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_AIRPORT_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_DATA_FACILITY_AIRPORT)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_WAYPOINT_LIST

    // used by SIMCONNECT_RECV_WAYPOINT_LIST
    public static CX.SIMCONNECT_DATA_FACILITY_WAYPOINT GetFrom( FS.SIMCONNECT_DATA_FACILITY_WAYPOINT orig )
    {
      return new CX.SIMCONNECT_DATA_FACILITY_WAYPOINT( ) {
        Ident = orig.Ident,
        Region = orig.Region,
        Altitude = orig.Altitude,
        Latitude = orig.Latitude,
        Longitude = orig.Longitude,
        fMagVar = orig.fMagVar,
      };
    }
    public static CX.SIMCONNECT_RECV_WAYPOINT_LIST GetFrom( FS.SIMCONNECT_RECV_WAYPOINT_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_WAYPOINT_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_DATA_FACILITY_WAYPOINT)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_NDB_LIST

    // used by SIMCONNECT_RECV_NDB_LIST
    public static CX.SIMCONNECT_DATA_FACILITY_NDB GetFrom( FS.SIMCONNECT_DATA_FACILITY_NDB orig )
    {
      return new CX.SIMCONNECT_DATA_FACILITY_NDB( ) {
        Ident = orig.Ident,
        Region = orig.Region,
        Altitude = orig.Altitude,
        Latitude = orig.Latitude,
        Longitude = orig.Longitude,
        fMagVar = orig.fMagVar,
        fFrequency = orig.fFrequency,
      };
    }
    public static CX.SIMCONNECT_RECV_NDB_LIST GetFrom( FS.SIMCONNECT_RECV_NDB_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_NDB_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_DATA_FACILITY_NDB)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_VOR_LIST

    // used by SIMCONNECT_RECV_VOR_LIST
    public static CX.SIMCONNECT_DATA_FACILITY_VOR GetFrom( FS.SIMCONNECT_DATA_FACILITY_VOR orig )
    {
      return new CX.SIMCONNECT_DATA_FACILITY_VOR( ) {
        Ident = orig.Ident,
        Region = orig.Region,
        Altitude = orig.Altitude,
        Latitude = orig.Latitude,
        Longitude = orig.Longitude,
        fMagVar = orig.fMagVar,
        fFrequency = orig.fFrequency,
        fGlideSlopeAngle = orig.fGlideSlopeAngle,
        Flags = orig.Flags,
        fLocalizer = orig.fLocalizer,
        GlideAlt = orig.GlideAlt,
        GlideLat = orig.GlideLat,
        GlideLon = orig.GlideLon,
      };
    }
    public static CX.SIMCONNECT_RECV_VOR_LIST GetFrom( FS.SIMCONNECT_RECV_VOR_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_VOR_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_DATA_FACILITY_VOR)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS

    // used by SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS
    public static CX.SIMCONNECT_INPUT_EVENT_DESCRIPTOR GetFrom( FS.SIMCONNECT_INPUT_EVENT_DESCRIPTOR orig )
    {
      return new CX.SIMCONNECT_INPUT_EVENT_DESCRIPTOR( ) {
        eType = (CX.SIMCONNECT_INPUT_EVENT_TYPE)orig.eType,
        Hash = orig.Hash,
        Name = orig.Name,
      };
    }
    public static CX.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS GetFrom( FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS orig )
    {
      var ret = new CX.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_INPUT_EVENT_DESCRIPTOR)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_CONTROLLERS_LIST

    // used by SIMCONNECT_RECV_CONTROLLERS_LIST
    public static CX.SIMCONNECT_CONTROLLER_ITEM GetFrom( FS.SIMCONNECT_CONTROLLER_ITEM orig )
    {
      var ret = new CX.SIMCONNECT_CONTROLLER_ITEM( ) {
        DeviceName = orig.DeviceName,
        ProductId = orig.ProductId,
        CompositeID = orig.CompositeID,
        DeviceId = orig.DeviceId,
      };
      // orig.HardwareVersion is not visible in the IDE also
      // accessing orig.HardwareVersion.Build raises:
      //   CS0570 'SIMCONNECT_CONTROLLER_ITEM.HardwareVersion' is not supported by the language
      // so we leave HardwareVersion alone for now
      // ret.HardwareVersion.Build= orig.HardwareVersion.Build;
      return ret;
    }
    public static CX.SIMCONNECT_RECV_CONTROLLERS_LIST GetFrom( FS.SIMCONNECT_RECV_CONTROLLERS_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_CONTROLLERS_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_CONTROLLER_ITEM)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_JETWAY_DATA

    // used by SIMCONNECT_RECV_JETWAY_DATA
    public static CX.SIMCONNECT_JETWAY_DATA GetFrom( FS.SIMCONNECT_JETWAY_DATA orig )
    {
      return new CX.SIMCONNECT_JETWAY_DATA( ) {
        AirportIcao = orig.AirportIcao,
        ParkingIndex = orig.ParkingIndex,
        Lla = GetFrom( orig.Lla ),
        Pbh = GetFrom( orig.Pbh ),
        Status = orig.Status,
        Door = orig.Door,
        ExitDoorRelativePos = GetFrom( orig.ExitDoorRelativePos ),
        MainHandlePos = GetFrom( orig.MainHandlePos ),
        SecondaryHandle = GetFrom( orig.SecondaryHandle ),
        WheelGroundLock = GetFrom( orig.WheelGroundLock ),
        JetwayObjectId = orig.JetwayObjectId,
        AttachedObjectId = orig.AttachedObjectId,
      };
    }
    public static CX.SIMCONNECT_RECV_JETWAY_DATA GetFrom( FS.SIMCONNECT_RECV_JETWAY_DATA orig )
    {
      var ret = new CX.SIMCONNECT_RECV_JETWAY_DATA( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_JETWAY_DATA)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

    #region SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST (FS2024 only)

    // used by SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST
    public static CX.SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY GetFrom( FS.SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY orig )
    {
      return new CX.SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY( ) {
        AircraftTitle = orig.AircraftTitle,
        LiveryName = orig.LiveryName,
      };
    }
    public static CX.SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST GetFrom( FS.SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST orig )
    {
      var ret = new CX.SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (FS.SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY)orig.rgData[i] );
      }

      return ret;
    }

    #endregion

  }
}
