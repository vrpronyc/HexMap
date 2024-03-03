using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NavigationController;

public class NavigationController : MonoBehaviour
{
    // The m_TravelIsPossible table holds bit flags that show whether travel from a
    // hex subtriangle out an edge is possible.
    //
    //                   1
    //                -------
    //  edge # -> 0 /  \ 1 /  \  2
    // subtriangle -> 0 \ / 2  \
    //             ------+------
    //             \  5 / \ 3  /
    //            5 \  / 4 \  /  3
    //                -------
    //                   4
    //
    //     There is a byte for each subtriangle.
    //     Each bit of the byte, from bit 0 to bit 5, is 1 if travel is possible.  Otherwise 0.
    //

    byte[][] m_TravelIsPossible = new byte[][]
    {
       /* Hex 0*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 1*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 2*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 3*/ new byte[]
       {
          /* tri 0 */ 0x3E,
          /* tri 1 */ 0x3E,
          /* tri 2 */ 0x3E,
          /* tri 3 */ 0x3E,
          /* tri 4 */ 0x3E,
          /* tri 5 */ 0x3E,
       },
       /* Hex 4*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 5*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 6*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 7*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x3E,
          /* tri 2 */ 0x3E,
          /* tri 3 */ 0x3E,
          /* tri 4 */ 0x3E,
          /* tri 5 */ 0x3E,
       },
       /* Hex 8*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 9*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 10*/ new byte[]
       {
          /* tri 0 */ 0x3D,
          /* tri 1 */ 0x3D,
          /* tri 2 */ 0x3D,
          /* tri 3 */ 0x3D,
          /* tri 4 */ 0x3D,
          /* tri 5 */ 0x3D,
       },
       /* Hex 11*/ new byte[]
       {
          /* tri 0 */ 0x3C,
          /* tri 1 */ 0x3C,
          /* tri 2 */ 0x3C,
          /* tri 3 */ 0x3C,
          /* tri 4 */ 0x3C,
          /* tri 5 */ 0x3C,
       },
       /* Hex 12*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 13*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x3C,
          /* tri 3 */ 0x3C,
          /* tri 4 */ 0x3C,
          /* tri 5 */ 0x3C,
       },
       /* Hex 14*/ new byte[]
       {
          /* tri 0 */ 0x3D,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x3D,
          /* tri 3 */ 0x3D,
          /* tri 4 */ 0x3D,
          /* tri 5 */ 0x3D,
       },
       /* Hex 15*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x3C,
          /* tri 3 */ 0x3C,
          /* tri 4 */ 0x3C,
          /* tri 5 */ 0x3C,
       },
       /* Hex 16*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 17*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 18*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 19*/ new byte[]
       {
          /* tri 0 */ 0x3E,
          /* tri 1 */ 0x3E,
          /* tri 2 */ 0x3E,
          /* tri 3 */ 0x3E,
          /* tri 4 */ 0x3E,
          /* tri 5 */ 0x3E,
       },
       /* Hex 20*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 21*/ new byte[]
       {
          /* tri 0 */ 0x07,
          /* tri 1 */ 0x07,
          /* tri 2 */ 0x07,
          /* tri 3 */ 0x38,
          /* tri 4 */ 0x38,
          /* tri 5 */ 0x38,
       },
       /* Hex 22*/ new byte[]
       {
          /* tri 0 */ 0x39,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x39,
          /* tri 4 */ 0x39,
          /* tri 5 */ 0x39,
       },
       /* Hex 23*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x38,
          /* tri 4 */ 0x38,
          /* tri 5 */ 0x38,
       },
       /* Hex 24*/ new byte[]
       {
          /* tri 0 */ 0x3B,
          /* tri 1 */ 0x3B,
          /* tri 2 */ 0x3B,
          /* tri 3 */ 0x3B,
          /* tri 4 */ 0x3B,
          /* tri 5 */ 0x3B,
       },
       /* Hex 25*/ new byte[]
       {
          /* tri 0 */ 0x3B,
          /* tri 1 */ 0x3B,
          /* tri 2 */ 0x3B,
          /* tri 3 */ 0x3B,
          /* tri 4 */ 0x3B,
          /* tri 5 */ 0x3B,
       },
       /* Hex 26*/ new byte[]
       {
          /* tri 0 */ 0x39,
          /* tri 1 */ 0x39,
          /* tri 2 */ 0x39,
          /* tri 3 */ 0x39,
          /* tri 4 */ 0x39,
          /* tri 5 */ 0x39,
       },
       /* Hex 27*/ new byte[]
       {
          /* tri 0 */ 0x38,
          /* tri 1 */ 0x38,
          /* tri 2 */ 0x38,
          /* tri 3 */ 0x38,
          /* tri 4 */ 0x38,
          /* tri 5 */ 0x38,
       },
       /* Hex 28*/ new byte[]
       {
          /* tri 0 */ 0x3B,
          /* tri 1 */ 0x3B,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x3B,
          /* tri 4 */ 0x3B,
          /* tri 5 */ 0x3B,
       },
       /* Hex 29*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x38,
          /* tri 4 */ 0x38,
          /* tri 5 */ 0x38,
       },
       /* Hex 30*/ new byte[]
       {
          /* tri 0 */ 0x39,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x39,
          /* tri 4 */ 0x39,
          /* tri 5 */ 0x39,
       },
       /* Hex 31*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x38,
          /* tri 4 */ 0x38,
          /* tri 5 */ 0x38,
       },
       /* Hex 32*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 33*/ new byte[]
       {
          /* tri 0 */ 0x1F,
          /* tri 1 */ 0x1F,
          /* tri 2 */ 0x1F,
          /* tri 3 */ 0x1F,
          /* tri 4 */ 0x1F,
          /* tri 5 */ 0x1F,
       },
       /* Hex 34*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 35*/ new byte[]
       {
          /* tri 0 */ 0x1E,
          /* tri 1 */ 0x1E,
          /* tri 2 */ 0x1E,
          /* tri 3 */ 0x1E,
          /* tri 4 */ 0x1E,
          /* tri 5 */ 0x1E,
       },
       /* Hex 36*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 37*/ new byte[]
       {
          /* tri 0 */ 0x1F,
          /* tri 1 */ 0x1F,
          /* tri 2 */ 0x1F,
          /* tri 3 */ 0x1F,
          /* tri 4 */ 0x1F,
          /* tri 5 */ 0x00,
       },
       /* Hex 38*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x1E,
          /* tri 2 */ 0x1E,
          /* tri 3 */ 0x1E,
          /* tri 4 */ 0x1E,
          /* tri 5 */ 0x21,
       },
       /* Hex 39*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x1E,
          /* tri 2 */ 0x1E,
          /* tri 3 */ 0x1E,
          /* tri 4 */ 0x1E,
          /* tri 5 */ 0x00,
       },
       /* Hex 40*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 41*/ new byte[]
       {
          /* tri 0 */ 0x1F,
          /* tri 1 */ 0x1F,
          /* tri 2 */ 0x1F,
          /* tri 3 */ 0x1F,
          /* tri 4 */ 0x1F,
          /* tri 5 */ 0x1F,
       },
       /* Hex 42*/ new byte[]
       {
          /* tri 0 */ 0x3D,
          /* tri 1 */ 0x3D,
          /* tri 2 */ 0x3D,
          /* tri 3 */ 0x3D,
          /* tri 4 */ 0x3D,
          /* tri 5 */ 0x3D,
       },
       /* Hex 43*/ new byte[]
       {
          /* tri 0 */ 0x1C,
          /* tri 1 */ 0x1C,
          /* tri 2 */ 0x1C,
          /* tri 3 */ 0x1C,
          /* tri 4 */ 0x1C,
          /* tri 5 */ 0x1C,
       },
       /* Hex 44*/ new byte[]
       {
          /* tri 0 */ 0x23,
          /* tri 1 */ 0x23,
          /* tri 2 */ 0x1C,
          /* tri 3 */ 0x1C,
          /* tri 4 */ 0x1C,
          /* tri 5 */ 0x23,
       },
       /* Hex 45*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x1C,
          /* tri 3 */ 0x1C,
          /* tri 4 */ 0x1C,
          /* tri 5 */ 0x00,
       },
       /* Hex 46*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x1C,
          /* tri 3 */ 0x1C,
          /* tri 4 */ 0x1C,
          /* tri 5 */ 0x21,
       },
       /* Hex 47*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x1C,
          /* tri 3 */ 0x1C,
          /* tri 4 */ 0x1C,
          /* tri 5 */ 0x00,
       },
       /* Hex 48*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 49*/ new byte[]
       {
          /* tri 0 */ 0x1F,
          /* tri 1 */ 0x1F,
          /* tri 2 */ 0x1F,
          /* tri 3 */ 0x1F,
          /* tri 4 */ 0x1F,
          /* tri 5 */ 0x1F,
       },
       /* Hex 50*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 51*/ new byte[]
       {
          /* tri 0 */ 0x1E,
          /* tri 1 */ 0x1E,
          /* tri 2 */ 0x1E,
          /* tri 3 */ 0x1E,
          /* tri 4 */ 0x1E,
          /* tri 5 */ 0x1E,
       },
       /* Hex 52*/ new byte[]
       {
          /* tri 0 */ 0x27,
          /* tri 1 */ 0x27,
          /* tri 2 */ 0x27,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x27,
       },
       /* Hex 53*/ new byte[]
       {
          /* tri 0 */ 0x07,
          /* tri 1 */ 0x07,
          /* tri 2 */ 0x07,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x00,
       },
       /* Hex 54*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x21,
       },
       /* Hex 55*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x00,
       },
       /* Hex 56*/ new byte[]
       {
          /* tri 0 */ 0x3B,
          /* tri 1 */ 0x3B,
          /* tri 2 */ 0x3B,
          /* tri 3 */ 0x3B,
          /* tri 4 */ 0x3B,
          /* tri 5 */ 0x3B,
       },
       /* Hex 57*/ new byte[]
       {
          /* tri 0 */ 0x1B,
          /* tri 1 */ 0x1B,
          /* tri 2 */ 0x1B,
          /* tri 3 */ 0x1B,
          /* tri 4 */ 0x1B,
          /* tri 5 */ 0x1B,
       },
       /* Hex 58*/ new byte[]
       {
          /* tri 0 */ 0x39,
          /* tri 1 */ 0x39,
          /* tri 2 */ 0x39,
          /* tri 3 */ 0x39,
          /* tri 4 */ 0x39,
          /* tri 5 */ 0x39,
       },
       /* Hex 59*/ new byte[]
       {
          /* tri 0 */ 0x18,
          /* tri 1 */ 0x18,
          /* tri 2 */ 0x18,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x18,
       },
       /* Hex 60*/ new byte[]
       {
          /* tri 0 */ 0x23,
          /* tri 1 */ 0x23,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x23,
       },
       /* Hex 61*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x00,
       },
       /* Hex 62*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x21,
       },
       /* Hex 63*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x18,
          /* tri 4 */ 0x18,
          /* tri 5 */ 0x00,
       },
       /* Hex 64*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 65*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 66*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 67*/ new byte[]
       {
          /* tri 0 */ 0x3E,
          /* tri 1 */ 0x3E,
          /* tri 2 */ 0x3E,
          /* tri 3 */ 0x3E,
          /* tri 4 */ 0x3E,
          /* tri 5 */ 0x3E,
       },
       /* Hex 68*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 69*/ new byte[]
       {
          /* tri 0 */ 0x0F,
          /* tri 1 */ 0x0F,
          /* tri 2 */ 0x0F,
          /* tri 3 */ 0x0F,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 70*/ new byte[]
       {
          /* tri 0 */ 0x31,
          /* tri 1 */ 0x0E,
          /* tri 2 */ 0x0E,
          /* tri 3 */ 0x0E,
          /* tri 4 */ 0x31,
          /* tri 5 */ 0x31,
       },
       /* Hex 71*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x0E,
          /* tri 2 */ 0x0E,
          /* tri 3 */ 0x0E,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 72*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 73*/ new byte[]
       {
          /* tri 0 */ 0x3F,
          /* tri 1 */ 0x3F,
          /* tri 2 */ 0x3F,
          /* tri 3 */ 0x3F,
          /* tri 4 */ 0x3F,
          /* tri 5 */ 0x3F,
       },
       /* Hex 74*/ new byte[]
       {
          /* tri 0 */ 0x3D,
          /* tri 1 */ 0x3D,
          /* tri 2 */ 0x3D,
          /* tri 3 */ 0x3D,
          /* tri 4 */ 0x3D,
          /* tri 5 */ 0x3D,
       },
       /* Hex 75*/ new byte[]
       {
          /* tri 0 */ 0x3C,
          /* tri 1 */ 0x3C,
          /* tri 2 */ 0x3C,
          /* tri 3 */ 0x3C,
          /* tri 4 */ 0x3C,
          /* tri 5 */ 0x3C,
       },
       /* Hex 76*/ new byte[]
       {
          /* tri 0 */ 0x33,
          /* tri 1 */ 0x33,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x33,
          /* tri 5 */ 0x33,
       },
       /* Hex 77*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 78*/ new byte[]
       {
          /* tri 0 */ 0x31,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x31,
          /* tri 5 */ 0x31,
       },
       /* Hex 79*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 80*/ new byte[]
       {
          /* tri 0 */ 0x37,
          /* tri 1 */ 0x37,
          /* tri 2 */ 0x37,
          /* tri 3 */ 0x37,
          /* tri 4 */ 0x37,
          /* tri 5 */ 0x37,
       },
       /* Hex 81*/ new byte[]
       {
          /* tri 0 */ 0x37,
          /* tri 1 */ 0x37,
          /* tri 2 */ 0x37,
          /* tri 3 */ 0x37,
          /* tri 4 */ 0x37,
          /* tri 5 */ 0x37,
       },
       /* Hex 82*/ new byte[]
       {
          /* tri 0 */ 0x37,
          /* tri 1 */ 0x37,
          /* tri 2 */ 0x37,
          /* tri 3 */ 0x37,
          /* tri 4 */ 0x37,
          /* tri 5 */ 0x37,
       },
       /* Hex 83*/ new byte[]
       {
          /* tri 0 */ 0x36,
          /* tri 1 */ 0x36,
          /* tri 2 */ 0x36,
          /* tri 3 */ 0x36,
          /* tri 4 */ 0x36,
          /* tri 5 */ 0x36,
       },
       /* Hex 84*/ new byte[]
       {
          /* tri 0 */ 0x37,
          /* tri 1 */ 0x37,
          /* tri 2 */ 0x37,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x37,
          /* tri 5 */ 0x37,
       },
       /* Hex 85*/ new byte[]
       {
          /* tri 0 */ 0x07,
          /* tri 1 */ 0x07,
          /* tri 2 */ 0x07,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 86*/ new byte[]
       {
          /* tri 0 */ 0x31,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x31,
          /* tri 5 */ 0x31,
       },
       /* Hex 87*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 88*/ new byte[]
       {
          /* tri 0 */ 0x33,
          /* tri 1 */ 0x33,
          /* tri 2 */ 0x33,
          /* tri 3 */ 0x33,
          /* tri 4 */ 0x33,
          /* tri 5 */ 0x33,
       },
       /* Hex 89*/ new byte[]
       {
          /* tri 0 */ 0x33,
          /* tri 1 */ 0x33,
          /* tri 2 */ 0x33,
          /* tri 3 */ 0x33,
          /* tri 4 */ 0x33,
          /* tri 5 */ 0x33,
       },
       /* Hex 90*/ new byte[]
       {
          /* tri 0 */ 0x31,
          /* tri 1 */ 0x31,
          /* tri 2 */ 0x31,
          /* tri 3 */ 0x31,
          /* tri 4 */ 0x31,
          /* tri 5 */ 0x31,
       },
       /* Hex 91*/ new byte[]
       {
          /* tri 0 */ 0x30,
          /* tri 1 */ 0x30,
          /* tri 2 */ 0x30,
          /* tri 3 */ 0x30,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 92*/ new byte[]
       {
          /* tri 0 */ 0x33,
          /* tri 1 */ 0x33,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x33,
          /* tri 5 */ 0x33,
       },
       /* Hex 93*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 94*/ new byte[]
       {
          /* tri 0 */ 0x31,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x31,
          /* tri 5 */ 0x31,
       },
       /* Hex 95*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x30,
          /* tri 5 */ 0x30,
       },
       /* Hex 96*/ new byte[]
       {
          /* tri 0 */ 0x2F,
          /* tri 1 */ 0x2F,
          /* tri 2 */ 0x2F,
          /* tri 3 */ 0x2F,
          /* tri 4 */ 0x2F,
          /* tri 5 */ 0x2F,
       },
       /* Hex 97*/ new byte[]
       {
          /* tri 0 */ 0x0F,
          /* tri 1 */ 0x0F,
          /* tri 2 */ 0x0F,
          /* tri 3 */ 0x0F,
          /* tri 4 */ 0x0F,
          /* tri 5 */ 0x0F,
       },
       /* Hex 98*/ new byte[]
       {
          /* tri 0 */ 0x2F,
          /* tri 1 */ 0x2F,
          /* tri 2 */ 0x2F,
          /* tri 3 */ 0x2F,
          /* tri 4 */ 0x2F,
          /* tri 5 */ 0x2F,
       },
       /* Hex 99*/ new byte[]
       {
          /* tri 0 */ 0x0E,
          /* tri 1 */ 0x0E,
          /* tri 2 */ 0x0E,
          /* tri 3 */ 0x0E,
          /* tri 4 */ 0x0E,
          /* tri 5 */ 0x0E,
       },
       /* Hex 100*/ new byte[]
       {
          /* tri 0 */ 0x2F,
          /* tri 1 */ 0x2F,
          /* tri 2 */ 0x2F,
          /* tri 3 */ 0x2F,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x2F,
       },
       /* Hex 101*/ new byte[]
       {
          /* tri 0 */ 0x0F,
          /* tri 1 */ 0x0F,
          /* tri 2 */ 0x0F,
          /* tri 3 */ 0x0F,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 102*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x0E,
          /* tri 2 */ 0x0E,
          /* tri 3 */ 0x0E,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x21,
       },
       /* Hex 103*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x0E,
          /* tri 2 */ 0x0E,
          /* tri 3 */ 0x0E,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 104*/ new byte[]
       {
          /* tri 0 */ 0x2F,
          /* tri 1 */ 0x2F,
          /* tri 2 */ 0x2F,
          /* tri 3 */ 0x2F,
          /* tri 4 */ 0x2F,
          /* tri 5 */ 0x2F,
       },
       /* Hex 105*/ new byte[]
       {
          /* tri 0 */ 0x0F,
          /* tri 1 */ 0x0F,
          /* tri 2 */ 0x0F,
          /* tri 3 */ 0x0F,
          /* tri 4 */ 0x0F,
          /* tri 5 */ 0x0F,
       },
       /* Hex 106*/ new byte[]
       {
          /* tri 0 */ 0x2D,
          /* tri 1 */ 0x2D,
          /* tri 2 */ 0x2D,
          /* tri 3 */ 0x2D,
          /* tri 4 */ 0x2D,
          /* tri 5 */ 0x2D,
       },
       /* Hex 107*/ new byte[]
       {
          /* tri 0 */ 0x0C,
          /* tri 1 */ 0x0C,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x0C,
          /* tri 5 */ 0x0C,
       },
       /* Hex 108*/ new byte[]
       {
          /* tri 0 */ 0x23,
          /* tri 1 */ 0x23,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x23,
       },
       /* Hex 109*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 110*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x21,
       },
       /* Hex 111*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x0C,
          /* tri 3 */ 0x0C,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 112*/ new byte[]
       {
          /* tri 0 */ 0x27,
          /* tri 1 */ 0x27,
          /* tri 2 */ 0x27,
          /* tri 3 */ 0x27,
          /* tri 4 */ 0x27,
          /* tri 5 */ 0x27,
       },
       /* Hex 113*/ new byte[]
       {
          /* tri 0 */ 0x07,
          /* tri 1 */ 0x07,
          /* tri 2 */ 0x07,
          /* tri 3 */ 0x07,
          /* tri 4 */ 0x07,
          /* tri 5 */ 0x07,
       },
       /* Hex 114*/ new byte[]
       {
          /* tri 0 */ 0x27,
          /* tri 1 */ 0x27,
          /* tri 2 */ 0x27,
          /* tri 3 */ 0x27,
          /* tri 4 */ 0x27,
          /* tri 5 */ 0x27,
       },
       /* Hex 115*/ new byte[]
       {
          /* tri 0 */ 0x06,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x06,
          /* tri 4 */ 0x06,
          /* tri 5 */ 0x06,
       },
       /* Hex 116*/ new byte[]
       {
          /* tri 0 */ 0x27,
          /* tri 1 */ 0x27,
          /* tri 2 */ 0x27,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x27,
       },
       /* Hex 117*/ new byte[]
       {
          /* tri 0 */ 0x07,
          /* tri 1 */ 0x07,
          /* tri 2 */ 0x07,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 118*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x21,
       },
       /* Hex 119*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x06,
          /* tri 2 */ 0x06,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 120*/ new byte[]
       {
          /* tri 0 */ 0x23,
          /* tri 1 */ 0x23,
          /* tri 2 */ 0x23,
          /* tri 3 */ 0x23,
          /* tri 4 */ 0x23,
          /* tri 5 */ 0x23,
       },
       /* Hex 121*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x03,
          /* tri 3 */ 0x03,
          /* tri 4 */ 0x03,
          /* tri 5 */ 0x03,
       },
       /* Hex 122*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x21,
          /* tri 2 */ 0x21,
          /* tri 3 */ 0x21,
          /* tri 4 */ 0x21,
          /* tri 5 */ 0x21,
       },
       /* Hex 123*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 124*/ new byte[]
       {
          /* tri 0 */ 0x23,
          /* tri 1 */ 0x23,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x23,
       },
       /* Hex 125*/ new byte[]
       {
          /* tri 0 */ 0x03,
          /* tri 1 */ 0x03,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
       /* Hex 126*/ new byte[]
       {
          /* tri 0 */ 0x21,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x21,
       },
       /* Hex 127*/ new byte[]
       {
          /* tri 0 */ 0x00,
          /* tri 1 */ 0x00,
          /* tri 2 */ 0x00,
          /* tri 3 */ 0x00,
          /* tri 4 */ 0x00,
          /* tri 5 */ 0x00,
       },
    };

    public enum HexMovementEffect { Undef, None, Discovery, Sink, Dock };
    static NavigationController m_instance;

    public enum NavigationControlType { Planning, Sailing, WaitingOnModal };
    NavigationControlType m_NavigationControlType = NavigationControlType.Planning;

    public static NavigationController Instance
    {
        get
        {
            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    public enum NavigationState { Undef, Plotting, Sailing };
    public NavigationState m_NavigationState = NavigationState.Undef;

    public ShipManager m_ShipManager;

    public GameObject m_HexPathPrefab;

    public float m_SailDwellTime = 1.0f;

    public class PathEntry
    {
        public Hex hex;
        public GameObject marker;
    }
    //List<PathEntry> m_Path;
    //public List<PathEntry> GetCurrentShipPath()
    //{
    //    if (GameController.Instance.m_CurrentShip == null)
    //    {
    //        return null;
    //    }
    //    return GameController.Instance.m_CurrentShip.GetPath(); 
    //}

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartPath(Hex hex, ShipManager shipManager)
    {
        //ClearPath(GameController.Instance.m_CurrentShip);
        //AddHexToPath(hex);
        ClearPath(shipManager);
        AddHexToPath(hex, shipManager);
        
    }
    bool HexIsValid(Hex hex)
    {
        if (GameController.Instance.m_CurrentShip == null)
        {
            return false;
        }
        List<NavigationController.PathEntry> path = GameController.Instance.m_CurrentShip.GetPath();
        if (hex == null)
        {
            return false;
        }    
        if (path.Count == 0)
        {
            return true;
        }
        Hex lastHex = path[path.Count - 1].hex;
        if (hex == lastHex)
        {
            return false;
        }
        for (int i = 0; i < lastHex.m_SeaNeighbor.Length; i++)
        {
            if (hex == lastHex.m_SeaNeighbor[i])
            {
                return true;
            }
        }
        return false;
    }
    void AddHexToPath(Hex hex, ShipManager shipManager)
    {
        if (shipManager == null)
        {
            return;
        }
        Transform parent = shipManager.transform;
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        if (HexIsValid(hex))
        {
            GameObject go = Instantiate(m_HexPathPrefab);
            go.layer = shipManager.m_ShipLayer;
            for (int j = 0; j < go.transform.childCount; j++)
            {
                go.transform.GetChild(j).gameObject.layer = go.layer;
            }
            HexPathController hpc = go.GetComponent<HexPathController>();
            if (hpc == null)
            {
                Debug.LogError("No HexPathController on HexPathPrefab");
                return;
            }
            go.transform.position = hex.GetHexPosition();
            go.transform.SetParent(parent, false);
            go.name = "path_" + path.Count.ToString();
            PathEntry entry = new PathEntry();
            entry.hex = hex;
            entry.marker = go;
            hpc.SetLabel(path.Count.ToString());
            path.Add(entry);
        }
    }
    public void ClearPathSteps(ShipManager shipManager, int iNumberOfSteps)
    {
        if (shipManager == null)
        {
            return;
        }
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        int start = Mathf.Min(iNumberOfSteps, path.Count);
        for (int i = start - 1; i >= 0; i--)
        {
            Destroy(path[i].marker);
        }
        path.RemoveRange(0, iNumberOfSteps); 
    }

    public void ClearCurrentShipPath()
    {
        if (GameController.Instance.m_CurrentShip != null)
        {
            List<PathEntry> path = GameController.Instance.m_CurrentShip.GetPath();
            Hex hex = path[0].hex;
            ClearPath(GameController.Instance.m_CurrentShip);
            AddHexToPath(hex, GameController.Instance.m_CurrentShip);
        }
    }

    public void DeleteCurrentShipLatest()
    {
        if (GameController.Instance.m_CurrentShip != null)
        {
            List<PathEntry> path = GameController.Instance.m_CurrentShip.GetPath();
            if (path.Count > 1)
            {
                Destroy(path[path.Count - 1].marker);
                path.RemoveAt(path.Count - 1);
            }
        }
    }
    public void ClearPath(ShipManager shipManager)
    {
        if (shipManager == null)
        {
            return;
        }
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        for (int i = path.Count - 1; i >= 0 ; i--)
        {
            Destroy(path[i].marker);
        }
        path.Clear();
    }

    public void Sail()
    {
        m_NavigationControlType = NavigationControlType.Sailing;
        StartCoroutine(SailCoroutine());
    }

    HexMovementEffect MoveToHex(ShipManager ship, Hex hex)
    {
        if (hex == null)
        {
            return HexMovementEffect.None;
        }
        Hex.HexVisibility visibility = hex.m_HexVisibility;
        if (hex.m_HexVisibility == Hex.HexVisibility.Unknown)
        {
            hex.SetHexVisibility(Hex.HexVisibility.Discovered);
            ship.AddToDiscoveredHexes(hex);
        }
        switch (hex.GetHexSubType())
        {
            case Hex.HexSubType.Undefined:
                break;
            case Hex.HexSubType.Home:
                return HexMovementEffect.Dock;
                break;
            case Hex.HexSubType.Waystation:
                return HexMovementEffect.Dock;
                break;
            case Hex.HexSubType.Hazard:
                return HexMovementEffect.Sink;
                break;
            default:
                break;
        }
        if ((visibility == Hex.HexVisibility.Unknown) 
            && hex.HexHasLand())
        {
            return HexMovementEffect.Discovery;
        }
        return HexMovementEffect.None;

    }
    IEnumerator SailCoroutine()
    {
        if (m_NavigationControlType == NavigationControlType.Sailing)
        {
            HexMovementEffect effect = HexMovementEffect.None;
            bool keepSailing = true; // If anything happens to any ship, we'll stop the coroutine
            int iDayCount = 0;
            Hex hex = null;
            while (keepSailing)
            {
                if (iDayCount > 0)
                {
                    GameController.Instance.IncrementDay();
                }
                for (int iShipIndex = 0; iShipIndex < GameController.Instance.m_ShipManagers.Count; iShipIndex++)
                {
                    ShipManager ship = GameController.Instance.m_ShipManagers[iShipIndex];
                    List<PathEntry> path = ship.GetPath();
                    if (path == null)
                    {
                        keepSailing = false;
                        Debug.LogError("NO PATH");
                        continue;
                    }
                    if (path.Count == 0)
                    {
                        keepSailing = false;
                        continue;
                    }
                    hex = path[0].hex;
                    effect = MoveToHex(ship, hex);
                    //if ((iDayCount > 0) && (effect != HexMovementEffect.None)) // day 0 is just a repeat of the prior route's last day
                    if (iDayCount > 0) // day 0 is just a repeat of the prior route's last day
                    {
                        //keepSailing = false;
                        //GameController.Instance.HandleMovementEffect(ship, hex, effect);
                        keepSailing = GameController.Instance.HandleMovementEffect(ship, hex, effect);
                    }

                    if ((path.Count == 1) || !keepSailing)
                    {
                        ClearPath(ship);
                        if (hex != null)
                        {
                            AddHexToPath(hex, ship);
                        }
                        keepSailing = false;
                    }
                    else
                    {
                        ClearPathSteps(ship, 1);
                    }

                }
                if (!keepSailing)
                {
                    break;
                }
                if (iDayCount > 0)
                {
                    yield return new WaitForSeconds(m_SailDwellTime);
                }
                iDayCount++;
            }
            Debug.Log("Done Sailing today");
            //if (effect != HexMovementEffect.None)
            //{
            //    GameController.Instance.HandleMovementEffect(hex, effect);
            //}
            m_NavigationControlType = NavigationControlType.Planning;
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
    }
    public void AddToCurrentShipPath()
    {
        if (m_NavigationControlType == NavigationControlType.Planning)
        {
            if (GameController.Instance.m_CurrentShip != null)
            {
                AddToPath(GameController.Instance.m_CurrentShip);
            }
        }
    }
    public void AddToPath(ShipManager shipManager)
    {
        if (HexMapBuilder.Instance == null)
        {
            return;
        }

        Hex hex = HexMapBuilder.Instance.FetchHexFromClickPoint();
        if (hex != null)
        {
            AddHexToPath(hex, shipManager);
        }
    }
}
