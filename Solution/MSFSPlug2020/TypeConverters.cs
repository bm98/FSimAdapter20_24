using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using FS = Microsoft.FlightSimulator.SimConnect;
using CX = MSFSAdapter20_24;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace MSFSPlug2020
{
  /// <summary>
  /// Explicite Type mapping mostly for lists where changes have been applied to each record
  /// 
  ///  Delta 2020 to 2024
  ///    SIMCONNECT_ICAO.Ident String length 6 vs 9
  ///    
  ///    SIMCONNECT_DATA_FACILITY_AIRPORT.Ident String length 6 vs 9
  ///    
  ///    SIMCONNECT_DATA_FACILITY_WAYPOINT.Ident String length 6 vs 9
  ///       SIMCONNECT_DATA_FACILITY_AIRPORT.Ident String length 6 vs 9
  ///       
  ///    SIMCONNECT_DATA_FACILITY_NDB.Ident String length 6 vs 9
  ///       SIMCONNECT_DATA_FACILITY_WAYPOINT.Ident String length 6 vs 9
  ///           SIMCONNECT_DATA_FACILITY_AIRPORT.Ident String length 6 vs 9
  ///           
  ///    SIMCONNECT_DATA_FACILITY_VOR.Ident String length 6 vs 9
  ///       SIMCONNECT_DATA_FACILITY_NDB.Ident String length 6 vs 9
  ///         SIMCONNECT_DATA_FACILITY_WAYPOINT.Ident String length 6 vs 9
  ///             SIMCONNECT_DATA_FACILITY_AIRPORT.Ident String length 6 vs 9
  /// </summary>
  internal static class TypeConverters
  {
    // needs Ident adjustment
    // used by SIMCONNECT_RECV_FACILITY_MINIMAL_LIST
    public static CX.SIMCONNECT_ICAO GetFrom( FS.SIMCONNECT_ICAO orig )
    {
      return new CX.SIMCONNECT_ICAO( ) {
        Airport = orig.Airport,
        Ident = orig.Ident,
        Region = orig.Region,
        Type = orig.Type
      };
    }

    // needs Ident adjustment
    // used by SIMCONNECT_RECV_FACILITY_MINIMAL_LIST
    public static CX.SIMCONNECT_FACILITY_MINIMAL GetFrom( FS.SIMCONNECT_FACILITY_MINIMAL orig )
    {
      return new CX.SIMCONNECT_FACILITY_MINIMAL( ) {
        icao = GetFrom( orig.icao ), // needs Ident adjustment
        lla = new CX.SIMCONNECT_DATA_LATLONALT( ) { Latitude = orig.lla.Latitude, Longitude = orig.lla.Longitude, Altitude = orig.lla.Altitude },
      };
    }
    // needs Ident adjustment due to using SIMCONNECT_FACILITY_MINIMAL
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
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }

    // needs Ident adjustment
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
    // needs Ident adjustment due to using SIMCONNECT_DATA_FACILITY_AIRPORT
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
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }


    // needs Ident adjustment
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
    // needs Ident adjustment due to using SIMCONNECT_DATA_FACILITY_WAYPOINT
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
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }

    // needs Ident adjustment
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
    // needs Ident adjustment due to using SIMCONNECT_DATA_FACILITY_NDB
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
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }


    // needs Ident adjustment
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
    // needs Ident adjustment due to using SIMCONNECT_DATA_FACILITY_VOR
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
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }

  }
}
