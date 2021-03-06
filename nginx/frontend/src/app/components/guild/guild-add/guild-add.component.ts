import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { Guild } from 'src/app/models/Guild';
import { GuildRole } from 'src/app/models/GuildRole';
import { MetaClientId } from 'src/app/models/MetaClientId';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-guild-add',
  templateUrl: './guild-add.component.html',
  styleUrls: ['./guild-add.component.scss']
})
export class GuildAddComponent implements OnInit {

  loading: boolean = true;

  displayGuildSelected: boolean = false;
  displayValidGuildSelected: boolean = false;
  displayGuildInvite: boolean = false;

  @Input() modRole!: string;
  @Input() adminRole!: string;
  @Input() mutedRole!: string;
  @Input() internalWebhook!: string;
  @Input() publicWebhook!: string;

  selectedGuildId: string;
  selectedGuild: Promise<Guild>;
  guilds: Promise<Guild[]>;
  clientid: Promise<MetaClientId>;

  constructor(private api: ApiService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute, private authService: AuthService) { }

  ngOnInit(): void {
    this.guilds = this.api.getSimpleData('/discord/guilds').toPromise();
    this.clientid = this.api.getSimpleData('/meta/clientid').toPromise();

    this.guilds.then(() => {
      this.loading = false;
      if (this.route.snapshot.queryParamMap.get('guildid')) {
        this.selectedGuildId = this.route.snapshot.queryParamMap.get('guildid');
        this.onGuildSelect();
      }
    })
  }

  isNumber(n: any): boolean { return !isNaN(parseFloat(n)) && !isNaN(n - 0) }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  onGuildSelect() {
    if (this.isNumber(this.selectedGuildId)) {
      this.displayGuildSelected = true;
      this.selectedGuild = this.api.getSimpleData(`/discord/guilds/${this.selectedGuildId}`).toPromise().then((data) => {
        this.displayGuildInvite = false;
        this.displayValidGuildSelected = true;
        return data;
      }).catch((error) => {
        this.displayGuildInvite = true;
        this.displayValidGuildSelected = false;
      });
    } else {
      this.displayGuildSelected = false;
      this.displayValidGuildSelected = false;
    }
  }

  registerGuild() {
    let data = {
      guildid: this.selectedGuildId,
      modroleid: this.modRole,
      adminroleid: this.adminRole,
      mutedroleid: this.isNumber(this.mutedRole) ? this.mutedRole : null,
      modInternalNotificationWebhook: this.internalWebhook ? this.internalWebhook.match(/^ *$/) ? null : this.internalWebhook : null,
      modPublicNotificationWebhook: this.publicWebhook ? this.publicWebhook.match(/^ *$/) ? null : this.publicWebhook : null
    };
    this.api.postSimpleData('/guilds/', data).subscribe((data) => {
      this.toastr.success('Guild created');
      this.authService.resetCache();
      this.router.navigate(['guilds']);
    }, (error) => {
      this.toastr.error('Cannot register guild.', 'Something went wrong.');
    })
  }
}
