{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
  nativeBuildInputs = with pkgs; [ mono dotnet-sdk ];

  shellHook = ''
    dotnet build
    mono bin/Debug/BattleShips.exe
    exit
  '';
}

