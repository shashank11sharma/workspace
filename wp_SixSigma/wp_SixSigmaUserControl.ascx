<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wp_SixSigmaUserControl.ascx.cs"
    Inherits="PWC.Process.SixSigma.wp_SixSigma.wp_SixSigmaUserControl" %>
<link rel="stylesheet" href="/_layouts/15/PWC.Process.SixSigma/css/jquery-ui.css" />
<link rel="stylesheet" href="/_layouts/15/PWC.Process.SixSigma/css/formSixSigma.css" />
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery-1.11.0.min.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery.SPServices-0.7.1a.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery-ui.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/Modal.js" type="text/javascript"></script>
<script type="text/javascript">


    var GlobalLanguageId = 1;
    var siteURL;

    function OpenPopup(btnName) {
        SetTab(0);
        $("#tabs").tabs({ active: 0 });
        if (btnName == "BB") {
            var html = $("[id*=BlackbeltuserEditor]");
            var DocReviewerLoginName = $("#divEntityData", html).attr("key");
            var blackURL = siteURL + "/SitePages/AddGroupUsers.aspx?SigmaId=" + sigmaID + "&Group=BlackBelt&LanguageId=" + GlobalLanguageId + "&User=" + encodeURIComponent(DocReviewerLoginName);
            popupmodaluiReturnValue(blackURL, '');
        }
        else {
            var html = $("[id*=GreenbeltuserEditor]");
            var DocReviewerLoginName = $("#divEntityData", html).attr("key");
            var greenURL = siteURL + "/SitePages/AddGroupUsers.aspx?SigmaId=" + sigmaID + "&Group=GreenBelt&LanguageId=" + GlobalLanguageId + "&User=" + encodeURIComponent(DocReviewerLoginName);
            popupmodaluiReturnValue1(greenURL, '');
        }

        return false;
    }


    function setonLoad() {
        var GlobalLanguageId = culture;
        $("#tabs").tabs();
        //Added By SHiv for SP2016
        $("#titleAreaRow").hide();
        $("#suiteBarTop").hide();
        $("#pageContentTitle").hide();

        $('#s4-topheader2').hide();
        $('.header-secondrow').hide();
        $("#s4-ribboncont").hide();
        $("#s4-ribbonrow").hide();
        $("#s4-titlerow").hide();
        $("#s4-leftpanel").hide();
        limitText(800);
        disableDateTimeControl();
        var tabId = document.getElementById("<%=SelectedTab.ClientID %>").value;
        if (tabId == "1")
            $("#tabs").tabs({ active: 1 });
        else if (tabId == "2")
            $("#tabs").tabs({ active: 2 });
        else if (tabId == "3")
            $("#tabs").tabs({ active: 3 });
        else if (tabId == "4")
            $("#tabs").tabs({ active: 4 });

        $().SPServices({
            operation: "GetListItems",
            async: false,
            listName: "Lookup_ProcertMultilingual_List",
            completefunc: function (xData, Status) {
                $(xData.responseXML).SPFilterNode("z:row").each(function () {
                    //     alert(xData.responseText);
                    //                    if ($(this).attr("ows_LanguageValue") == 1) {
                    //                        ENgLanguage = $(this).attr("ows_LanguageValue");
                    setLanguage(culture);
                    // }

                });
            }
        });
    }

    $(document).ready(function () {
        setonLoad();
        siteURL = window.location.protocol + "//" + window.location.host + _spPageContextInfo.siteServerRelativeUrl;

    });

    function concurrentSavingError() {
        $("#tabs").tabs({ active: 0 });
        document.getElementById('<%= TrErrorLabelProjectTeam.ClientID %>').style.display = "";
        document.getElementById('<%= ProjectTeamErrorLabel.ClientID %>').innerHTML = 'Form is updated by other user - please close and re-open again.';
        return false;
    }

    function disableDateTimeControl() {
        $("[id*='ActualdateProjectAuthorization']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateProjectAuthorization']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateDefine']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateDefine']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateMeasure']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateMeasure']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateAnalyze']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateAnalyze']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateImprove']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateImprove']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateControl']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateControl']")[1].id).parent().removeAttr('onclick');

        $("[id*='ActualdateFinalReportApprove']")[0].readOnly = true;
        $("#" + $("[id*='ActualdateFinalReportApprove']")[1].id).parent().removeAttr('onclick');
    }
    function disableGates() {
        jQuery('#tabs').tabs('disable', 2);
    }

    function enableUpdatesAttachments() {
        jQuery('#tabs').tabs('enable', 1);
        jQuery('#tabs').tabs('enable', 3);
    }

    function SetTab(tabId) {
        document.getElementById("<%=SelectedTab.ClientID %>").value = tabId;
    }

    function disableThreeTabs() {
        jQuery('#tabs').tabs('disable', 1);
        jQuery('#tabs').tabs('disable', 2);
        jQuery('#tabs').tabs('disable', 3);
    }

    function checkPeopleEditor(PeopleEditor, RoleDropdown, departmentName, PercentageDropdown) {

        if (document.getElementById(PeopleEditor).value == "") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Project Team Memeber.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez l'équipe de projet Memeber.");
            }

            return false;
        }
        else if (document.getElementById(departmentName).value == "") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Project department name.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez Nom du projet du département.");
            }

            return false;
        }
        else if (document.getElementById(RoleDropdown).value == "0") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Project Team Role.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez l'équipe de projet Rôle.");
            }

            return false;
        }
        else if (document.getElementById(PercentageDropdown).value == "0") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Project Percentage.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez projet Pourcentage.");
            }

            return false;
        }
        else {
            return true;
        }
    }

    function checkAttachmentNameURL(LinkName, LinkURL) {

        if (document.getElementById(LinkName).value == "") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Link Name.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez Nom du lien.");
            }

            return false;
        }
        else if (document.getElementById(LinkURL).value == "") {
            if (GlobalLanguageId == 1) {
                alert("Please Enter Link URL.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît Entrez Lien URL.");
            }

            return false;
        }
        var regex = new RegExp("(https?:\/\/(?:www\.|(?!www))[^\s\.]+\.[^\s]{2,}|www\.[^\s]+\.[^\s]{2,})");
        if (regex.test(document.getElementById(LinkURL).value)) {
            return true;
        } else {
            if (GlobalLanguageId == 1) {
                alert("Please enter the valid URL.");
            }
            else if (GlobalLanguageId == 2) {
                alert("S'il vous plaît entrez l'URL valide.");
            }

            return false;
        }


    }

    function limitText(limitNum) {
        var ProjectComment = document.getElementById("<%=txtreturntooriginator.ClientID %>")
        var lf = document.getElementById("<%=txtBackground.ClientID %>")
        var background = document.getElementById("<%=txtProjectstatementobj.ClientID %>")
        var benefits = document.getElementById("<%=txtBenefits.ClientID %>")
        var costs = document.getElementById("<%=txtcosts.ClientID %>")
        var lc = document.getElementById("<%=txtCT1.ClientID %>")
        var lc1 = document.getElementById("<%=txtCT2.ClientID %>")
        var lc2 = document.getElementById("<%=txtCT3.ClientID %>")
        var lc3 = document.getElementById("<%=txtCT4.ClientID %>")
        var ProjectCommentCntdown = document.getElementById("<%=txtCountdown.ClientID %>")

        if (ProjectComment.value.length > limitNum) {
            ProjectComment.value = ProjectComment.value.substring(0, limitNum);
        } else {
            ProjectCommentCntdown.value = limitNum - ProjectComment.value.length;
        }
        if (lf.value.length > limitNum) {
            lf.value = lf.value.substring(0, limitNum);
        } else {
            lc.value = limitNum - lf.value.length;
        }

        if (background.value.length > limitNum) {
            background.value = background.value.substring(0, limitNum);
        } else {
            lc1.value = limitNum - background.value.length;
        }



        if (benefits.value.length > limitNum) {
            benefits.value = benefits.value.substring(0, limitNum);
        } else {
            lc2.value = limitNum - benefits.value.length;
        }



        if (costs.value.length > limitNum) {
            costs.value = costs.value.substring(0, limitNum);
        } else {
            lc3.value = limitNum - costs.value.length;
        }
    }

    function ProjectComments(btntext) {

        if (trimAll(document.getElementById("<%=txtProjectName.ClientID%>").value) == "") {
            $("#tabs").tabs({ active: 0 });
            document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
            if (GlobalLanguageId == 1) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Project name is mandatory.';
            }
            else if (GlobalLanguageId == 2) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Nom du projet est obligatoire.';
            }

        document.getElementById("<%=txtProjectName.ClientID%>").focus();
            return false;
        }
        var ddlOrganisation = document.getElementById("<%=ddlorgnisation.ClientID%>").value;
        if (ddlOrganisation == 0) {
            document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
            if (GlobalLanguageId == 1) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Organization Name is mandatory.';
            }
            else if (GlobalLanguageId == 2) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Nom du organisme est obligatoire.';
            }

        document.getElementById("<%= ddlorgnisation.ClientID %>").focus();
            return false;
        }

        var tags = document.getElementsByTagName("DIV");
        for (var i = 0; i < tags.length; i++) {
            var tempString = tags[i].id;
            if ((tempString.indexOf("projectSponserUserEditor") > 0) && (tempString.indexOf("projectSponserUserEditor_upLevelDiv") > 0)) {
                var innerSpans = tags[i].getElementsByTagName("SPAN");
                if (innerSpans.length == 0) {
                    document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Project Sponsor Name is mandatory.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Promoteur du projet Nom est obligatoire.';
                    }

                document.getElementById("<%=projectSponserUserEditor.ClientID%>").focus();
                    return false;
                }
            }
        }
        var ddlPlant = document.getElementById("<%=ddlplant.ClientID%>").value;
        if (ddlPlant == 0) {
            document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
            if (GlobalLanguageId == 1) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Plant Name is mandatory.';
            }
            else if (GlobalLanguageId == 2) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Nom du installation est obligatoire.';
            }

        document.getElementById("<%= ddlplant.ClientID %>").focus();
            return false;
        }



        var tags = document.getElementsByTagName("DIV");
        for (var i = 0; i < tags.length; i++) {
            var tempString = tags[i].id;
            if ((tempString.indexOf("BlackbeltuserEditor") > 0) && (tempString.indexOf("BlackbeltuserEditor_upLevelDiv") > 0)) {
                var innerSpans = tags[i].getElementsByTagName("SPAN");
                if (innerSpans.length == 0) {
                    document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Project Black Belt Name is mandatory.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Projet Black Belt Nom est obligatoire.';
                    }

                document.getElementById("<%=BlackbeltuserEditor.ClientID%>").focus();
                    return false;
                }
            }
        }
        var ddlProjectType = document.getElementById("<%=ddlprojecttype.ClientID%>").value;
        if (ddlProjectType == 0) {
            document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
            if (GlobalLanguageId == 1) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Project Type Name is mandatory.';
            }
            else if (GlobalLanguageId == 2) {
                document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Type de projet Nom est obligatoire.';
            }

        document.getElementById("<%= ddlprojecttype.ClientID %>").focus();
            return false;
        }

        var tags = document.getElementsByTagName("DIV");
        for (var i = 0; i < tags.length; i++) {
            var tempString = tags[i].id;
            if ((tempString.indexOf("GreenbeltuserEditor") > 0) && (tempString.indexOf("GreenbeltuserEditor_upLevelDiv") > 0)) {
                var innerSpans = tags[i].getElementsByTagName("SPAN");
                if (innerSpans.length == 0) {
                    document.getElementById('<%= TrErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Project Green Belt Name is mandatory.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectIdentificationErrorLabel.ClientID %>').innerHTML = 'Projet Ceinture verte Nom est obligatoire.';
                    }

                document.getElementById("<%=GreenbeltuserEditor.ClientID%>").focus();
                    return false;
                }
            }
        }




        if (btntext == "btnProjectAuthorization") {
            $("#tabs").tabs({ active: 0 });
            // document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "";
            //  document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "";
            //  document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "";
            //  document.getElementById('<%= CostsErrorLabel.ClientID %>').style.display = "";
            // document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "";
            // document.getElementById('<%= MilestonesErrorLabel.ClientID %>').style.display = "";

            if (trimAll(document.getElementById("<%=txtBackground.ClientID%>").value) == "") {
                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectBackGroundErrorLabel.ClientID %>').innerHTML = 'Project background is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectBackGroundErrorLabel.ClientID %>').innerHTML = 'Contexte du projet est obligatoire.';
                }

            document.getElementById("<%= txtBackground.ClientID %>").focus();
                return false;
            }
            else {
                var ProjectBackground = document.getElementById("<%=txtBackground.ClientID%>").value;
                var ProjectBackgroundLength = ProjectBackground.length;
                if (ProjectBackgroundLength > 800) {
                    document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectBackGroundErrorLabel.ClientID %>').innerHTML = 'Only 800 Characters allowed!.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectBackGroundErrorLabel.ClientID %>').innerHTML = 'Seuls 800 caractères autorisés!.';
                    }

                document.getElementById("<%= txtBackground.ClientID %>").focus();
                    return false;
                }
            }


            if (trimAll(document.getElementById("<%=txtProjectstatementobj.ClientID%>").value) == "") {
                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectPSOErrorLabel.ClientID %>').innerHTML = 'Project problem statement is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectPSOErrorLabel.ClientID %>').innerHTML = 'énoncé du problème du projet est obligatoire.';
                }

            document.getElementById("<%= txtProjectstatementobj.ClientID %>").focus();
                return false;
            }
            else {
                var ProjectPSO = document.getElementById("<%=txtProjectstatementobj.ClientID%>").value;
                var ProjectPSOLength = ProjectPSO.length;
                if (ProjectPSOLength > 800) {
                    document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectPSOErrorLabel.ClientID %>').innerHTML = 'Only 800 Characters allowed!.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectPSOErrorLabel.ClientID %>').innerHTML = 'Seuls 800 caractères autorisés!.';
                    }

                document.getElementById("<%= txtProjectstatementobj.ClientID %>").focus();
                    return false;
                }
            }

            if ((document.getElementById("<%=ddlothermetric.ClientID%>").value) != "0") {
            }
            else {
                if (document.getElementById("<%=ddlMetricCost.ClientID%>").value == "0" && trimAll(document.getElementById("<%=txtCostBaseline.ClientID%>").value) == "" && trimAll(document.getElementById("<%=txtCostGoal.ClientID%>").value) == "" && document.getElementById("<%=ddlQualityMetrics.ClientID%>").value == "0" && trimAll(document.getElementById("<%=txtQualityBaseline.ClientID%>").value) == "" && trimAll(document.getElementById("<%=txtQualityGoal.ClientID%>").value) == "" && document.getElementById("<%=ddlDeliveryMetrics.ClientID%>").value == "0" && trimAll(document.getElementById("<%=txtDeliveryBaseline.ClientID%>").value) == "" && trimAll(document.getElementById("<%=txtDeliveryGoal.ClientID%>").value) == "") {
                    document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectMetrErrorLabel.ClientID %>').innerHTML = 'Project metric statement is mandatory.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectMetrErrorLabel.ClientID %>').innerHTML = 'déclaration métrique projet est obligatoire.';
                    }

                document.getElementById("<%= ddlMetricCost.ClientID %>").focus();
                    return false;
                }
            }

            if (trimAll(document.getElementById("<%=txtBenefits.ClientID%>").value) == "") {
                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Planned financial analysis statement is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Prévues déclaration du analyse financière est obligatoire.';
                }

            document.getElementById("<%= txtBenefits.ClientID %>").focus();
                return false;
            }
            else {
                var ProjectBenefits = document.getElementById("<%=txtBenefits.ClientID%>").value;
                var ProjectBenefitsLength = ProjectBenefits.length;
                if (ProjectBenefitsLength > 800) {
                    document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Only 800 Characters allowed!.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Seuls 800 caractères autorisés!.';
                    }

                document.getElementById("<%= txtBenefits.ClientID %>").focus();
                    return false;
                }
            }

            if (trimAll(document.getElementById("<%=txtplannedActualCost.ClientID%>").value) == "") {


                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Planned cost  is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'coût prévu est obligatoire.';
                }

            document.getElementById("<%= txtplannedActualCost.ClientID %>").focus();
                return false;

            }

            if (trimAll(document.getElementById("<%=txtplannedActualBenefits.ClientID%>").value) == "") {


                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Planned benefits is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectBenefitsErrorLabel.ClientID %>').innerHTML = 'Avantages prévus est obligatoire.';
                }

            document.getElementById("<%= txtplannedActualBenefits.ClientID %>").focus();
                return false;

            }


            if (trimAll(document.getElementById("<%=txtcosts.ClientID%>").value) != "") {
                var ProjectCosts = document.getElementById("<%=txtcosts.ClientID%>").value;
                var ProjectCostsLength = ProjectCosts.length;
                if (ProjectCostsLength > 800) {
                    document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                    document.getElementById('<%= CostsErrorLabel.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ProjectCostsErrorLabel.ClientID %>').innerHTML = 'Only 800 Characters allowed!.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ProjectCostsErrorLabel.ClientID %>').innerHTML = 'Seuls 800 caractères autorisés!.';
                    }

                document.getElementById("<%= txtcosts.ClientID %>").focus();
                    return false;
                }
            }






            var Planned1 = document.getElementById('<%=PlandateProjectAuthorization.Controls[0].ClientID%>').value;
            var Planned2 = document.getElementById('<%=PlandateDefine.Controls[0].ClientID%>').value;
            var Planned3 = document.getElementById('<%=PlandateMeasure.Controls[0].ClientID%>').value;
            var Planned4 = document.getElementById('<%=PlandateAnalyze.Controls[0].ClientID%>').value;
            var Planned5 = document.getElementById('<%=PlandateImprove.Controls[0].ClientID%>').value;
            var Planned6 = document.getElementById('<%=PlandateControl.Controls[0].ClientID%>').value;
            var Planned7 = document.getElementById('<%=PlandateFinalReportApprove.Controls[0].ClientID%>').value;

            if (Planned1 == "" || Planned2 == "" || Planned3 == "" || Planned4 == "" || Planned5 == "" || Planned6 == "" || Planned7 == "") {
                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= CostsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";
                document.getElementById('<%= MilestonesErrorLabel.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectMilestonesErrorLabel.ClientID %>').innerHTML = 'Milestones statement is mandatory.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectMilestonesErrorLabel.ClientID %>').innerHTML = 'déclaration Jalons est obligatoire.';
                }

            return false;
        }

    }



    var GridProjectTeam = document.getElementById("<%=GridProjectTeam.ClientID %>");


        if (GridProjectTeam != null && GridProjectTeam.rows.length > 0) {
            var InnerText = GridProjectTeam.rows[1].cells[0].innerText;

            if (InnerText == " ") {
                $("#tabs").tabs({ active: 0 });
                document.getElementById('<%= BackgroudErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= PSOErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= BenefitsErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= CostsErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= ProjectMetricsErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= MilestonesErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= FinanceErrorLabel.ClientID %>').style.display = "none";

                document.getElementById('<%= TrErrorLabelProjectTeam.ClientID %>').style.display = "";

                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= ProjectTeamErrorLabel.ClientID %>').innerHTML = 'Please add at least one team member.';
                }
                else if (GlobalLanguageId == 2) {
                    document.getElementById('<%= ProjectTeamErrorLabel.ClientID %>').innerHTML = 'Sil vous plaît ajouter au moins un membre de équipe.';
                }

            return false;
        }
    }

    var commentFilled = true;
    document.getElementById('<%= TrDefineError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrFinalReportError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrControlError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrInvestigateError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrAnalyzeError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrMeasureError.ClientID %>').style.display = "none";
        document.getElementById('<%= TrDefineError.ClientID %>').style.display = "none";
        switch (btntext) {
            case 'btnDefineRequestApproval':
                if ($("[id*='txtdefineComment']")[0].value == "") {
                    document.getElementById('<%= TrDefineError.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= DefineErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= DefineErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
            break;
        case 'btnMeasureRequestApproval':
            if ($("[id*='txtMeasurecomment']")[0].value == "") {
                document.getElementById('<%= TrMeasureError.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= MeasureErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= MeasureErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
            break;
        case 'btnAnalyzeRequestApproval': if (
            $("[id*='txtAnalyzecomment']")[0].value == "") {
            document.getElementById('<%= TrAnalyzeError.ClientID %>').style.display = "";
                if (GlobalLanguageId == 1) {
                    document.getElementById('<%= AnalyzeErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= AnalyzeErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
                break;
            case 'btnInvestigateRequestApproval':
                if ($("[id*='txtinvestigatecomment']")[0].value == "") {
                    document.getElementById('<%= TrInvestigateError.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= InvestigateErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= InvestigateErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
            break;
        case 'btnControlRequestApproval':
            if ($("[id*='txtControlcomment']")[0].value == "") {
                document.getElementById('<%= TrControlError.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= ControlErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= ControlErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
            break;
        case 'btnFinalreportRequestApproval':
            if ($("[id*='txtFinalreportcomment']")[0].value == "") {
                document.getElementById('<%= TrFinalReportError.ClientID %>').style.display = "";
                    if (GlobalLanguageId == 1) {
                        document.getElementById('<%= FinalReportErrorLabel.ClientID %>').innerHTML = 'Comments are required before requesting approval.';
                    }
                    else if (GlobalLanguageId == 2) {
                        document.getElementById('<%= FinalReportErrorLabel.ClientID %>').innerHTML = 'Les commentaires sont nécessaires avant de demander approbation.';
                    }

                commentFilled = false;
            }
            break;
    }
    if (commentFilled) {
        changeCommentsViews(btntext);
    }

    return false;
}

function trimAll(sString) {
    while (sString.substring(0, 1) == ' ') {
        sString = sString.substring(1, sString.length);
    }
    while (sString.substring(sString.length - 1, sString.length) == ' ') {
        sString = sString.substring(0, sString.length - 1);
    }
    return sString;
}


function changeCommentsViews(flag) {
    document.getElementById('<%= TrReasonError.ClientID %>').style.display = "none";
        $("#tabs").toggle();
        $("#TableRC").toggle();
        switch (flag) {
            case 'btnProjectAuthorization': $("[id*='ProjectCommentsFlag']")[0].value = "btnProjectAuthorization";
                break;
            case 'btnSponsorApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnSponsorApproval";
                break;



            case 'btnlockform': $("[id*='ProjectCommentsFlag']")[0].value = "btnlockform";
                break;

            case 'btnUnlockform': $("[id*='ProjectCommentsFlag']")[0].value = "btnUnlockform";
                break;

            case 'btnEditCompleted': $("[id*='ProjectCommentsFlag']")[0].value = "btnEditCompleted";
                break;






            case 'btnBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnBBApproval";
                break;
            case 'btnReturnProjectLead': $("[id*='ProjectCommentsFlag']")[0].value = "btnReturnProjectLead";
                break;
            case 'btnDefineRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnDefineRequestApproval";
                break;
            case 'btnDefineBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnDefineBBApproval";
                break;
            case 'btnDefineReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnDefineReturntoProjectlead";
                break;
            case 'btnMeasureRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnMeasureRequestApproval";
                break;
            case 'btnMeasureBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnMeasureBBApproval";
                break;
            case 'btnMeasureReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnMeasureReturntoProjectlead";
                break;
            case 'btnAnalyzeRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnAnalyzeRequestApproval";
                break;
            case 'btnAnalyzeBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnAnalyzeBBApproval";
                break;
            case 'btnAnalyzeReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnAnalyzeReturntoProjectlead";
                break;
            case 'btnInvestigateRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnInvestigateRequestApproval";
                break;
            case 'btnInvestigateBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnInvestigateBBApproval";
                break;
            case 'btnInvestigateReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnInvestigateReturntoProjectlead";
                break;
            case 'btnControlRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnControlRequestApproval";
                break;
            case 'btnControlBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnControlBBApproval";
                break;
            case 'btnControlReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnControlReturntoProjectlead";
                break;
            case 'btnFinalreportRequestApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnFinalreportRequestApproval";
                break;
            case 'btnFinalreportBBApproval': $("[id*='ProjectCommentsFlag']")[0].value = "btnFinalreportBBApproval";
                break;
            case 'btnFinalreportReturntoProjectlead': $("[id*='ProjectCommentsFlag']")[0].value = "btnFinalreportReturntoProjectlead";
                break;
            case 'cancel': $("[id*='txtreturntooriginator']")[0].value = "";
                break;
        }
        return false;
    }



    function GetLanguage(select) {
        var ddlId = select.id;
        var language = $('#' + ddlId + ' :selected').val();
        if (language == "") {
            language = ENgLanguage;
        }
        document.getElementById('<%=HiddenLanguage.ClientID%>').value = language;

        GlobalLanguageId = language;
        setLanguage(language)
    }

    function setLanguage(language) {
        var languagequery = "<Query><Where><Eq><FieldRef Name='LanguageValue' /><Value Type='Number'>" + language + "</Value></Eq></Where></Query>";
        $().SPServices({
            debug: true,
            operation: "GetListItems",
            async: false,
            listName: "Lookup_ProcertMultilingual_List",
            CAMLQuery: languagequery,
            completefunc: function (xData, Status) {
                //     alert(xData.responseText);
                $(xData.responseXML).SPFilterNode("z:row").each(function () {

                    if (language == 1) {
                        var LG1 = $(this).attr("ows_Title");
                        $('#<%=ddlProcessForm1.ClientID %> option:contains(' + LG1 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm2.ClientID %> option:contains(' + LG1 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm3.ClientID %> option:contains(' + LG1 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm4.ClientID %> option:contains(' + LG1 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm5.ClientID %> option:contains(' + LG1 + ')').attr('selected', 'selected');
                    }
                    else if (language == 2) {
                        var LG2 = $(this).attr("ows_Title");
                        $('#<%=ddlProcessForm1.ClientID %> option:contains(' + LG2 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm2.ClientID %> option:contains(' + LG2 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm3.ClientID %> option:contains(' + LG2 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm4.ClientID %> option:contains(' + LG2 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm5.ClientID %> option:contains(' + LG2 + ')').attr('selected', 'selected');

                    }
                    else {
                        var LG3 = $(this).attr("ows_Title");
                        $('#<%=ddlProcessForm1.ClientID %> option:contains(' + LG3 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm2.ClientID %> option:contains(' + LG3 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm3.ClientID %> option:contains(' + LG3 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm4.ClientID %> option:contains(' + LG3 + ')').attr('selected', 'selected');
                        $('#<%=ddlProcessForm5.ClientID %> option:contains(' + LG3 + ')').attr('selected', 'selected');
                    }


                    $("[id*='tab1']").text($(this).attr("ows_LabelTab1"));
                    $("[id*='tab2']").text($(this).attr("ows_LabelTab2"));
                    $("[id*='tab3']").text($(this).attr("ows_LabelTab3"));
                    $("[id*='tab4']").text($(this).attr("ows_LabelTab4"));
                    $("[id*='tab5']").text($(this).attr("ows_LabelTab5"));

                    $("[id*='ProjectHeading']").text($(this).attr("ows_LabelProjectHeading"));
                    $("[id*='ProjectHeading1']").text($(this).attr("ows_LabelProjectHeading"));
                    $("[id*='ProjectHeading2']").text($(this).attr("ows_LabelProjectHeading"));
                    $("[id*='ProjectHeading3']").text($(this).attr("ows_LabelProjectHeading"));
                    $("[id*='ProjectHeading4']").text($(this).attr("ows_LabelProjectHeading"));

                    $("[id*='below']").text($(this).attr("ows_Labeldocument"));
                    $("[id*='below1']").text($(this).attr("ows_Labeldocument"));
                    $("[id*='below2']").text($(this).attr("ows_Labeldocument"));
                    $("[id*='below3']").text($(this).attr("ows_Labeldocument"));
                    $("[id*='below4']").text($(this).attr("ows_Labeldocument"));






                    $("[id*='lblProjectStatus1']").text($(this).attr("ows_LabelProjectStatus"));
                    $("[id*='lblProjectStatus2']").text($(this).attr("ows_LabelProjectStatus"));
                    $("[id*='lblProjectStatus3']").text($(this).attr("ows_LabelProjectStatus"));
                    $("[id*='lblProjectStatus4']").text($(this).attr("ows_LabelProjectStatus"));
                    $("[id*='lblProjectStatus5']").text($(this).attr("ows_LabelProjectStatus"));


                    $("[id*='lbllanguageid1']").text($(this).attr("ows_LabelLanguage"));
                    $("[id*='lbllanguageid2']").text($(this).attr("ows_LabelLanguage"));
                    $("[id*='lbllanguageid3']").text($(this).attr("ows_LabelLanguage"));
                    $("[id*='lbllanguageid4']").text($(this).attr("ows_LabelLanguage"));
                    $("[id*='lbllanguageid5']").text($(this).attr("ows_LabelLanguage"));



                    $("[id*='lblTextProjectId1']").text($(this).attr("ows_LabelProjectId"));
                    $("[id*='lblTextProjectId2']").text($(this).attr("ows_LabelProjectId"));
                    $("[id*='lblTextProjectId3']").text($(this).attr("ows_LabelProjectId"));
                    $("[id*='lblTextProjectId4']").text($(this).attr("ows_LabelProjectId"));
                    $("[id*='lblTextProjectId5']").text($(this).attr("ows_LabelProjectId"));



                    $("[id*='lblProjectName1']").text($(this).attr("ows_LabelProjectName"));
                    $("[id*='lblProjectName2']").text($(this).attr("ows_LabelProjectName"));
                    $("[id*='lblProjectName3']").text($(this).attr("ows_LabelProjectName"));
                    $("[id*='lblProjectName4']").text($(this).attr("ows_LabelProjectName"));
                    $("[id*='lblProjectName5']").text($(this).attr("ows_LabelProjectName"));


                    $("[id*='lblprojectinformation']").text($(this).attr("ows_LabelProjectInformation"));
                    $("[id*='lblprojectidentification']").text($(this).attr("ows_LabelProjectIdentification"));


                    $("[id*='lblnameproject']").text($(this).attr("ows_LabelProjName"));
                    $("[id*='lblorganization']").text($(this).attr("ows_LabelOrganization"));
                    $("[id*='lblprojectsponser']").text($(this).attr("ows_LabelSponser"));
                    $("[id*='lblprojectplant']").text($(this).attr("ows_LabelPlant"));
                    $("[id*='lblblackbeltusers']").text($(this).attr("ows_LabelBlackBelt"));
                    $("[id*='lblprojecttype']").text($(this).attr("ows_LabelProjectType"));
                    $("[id*='lblGreenbeltusers']").text($(this).attr("ows_LabelGreenBelt"));
                    $("[id*='lblTags']").text($(this).attr("ows_LabelTags"));

                    $("[id*='lblBackgroud']").text($(this).attr("ows_LabelBackground"));
                    $("[id*='lblsupportingdocuments1']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments2']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments3']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments4']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments5']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments6']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments7']").text($(this).attr("ows_LabelSupprtingdocuments"));
                    $("[id*='lblsupportingdocuments8']").text($(this).attr("ows_LabelSupprtingdocuments"));

                    $("[id*='lblsavesupportingdocs1']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs2']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs3']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs4']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs5']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs6']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs7']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs8']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs9']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs10']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs11']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs12']").text($(this).attr("ows_Labelsavesupportingdocs"));
                    $("[id*='lblsavesupportingdocs13']").text($(this).attr("ows_Labelsavesupportingdocs"));

                    $("[id*='lblproblemstatementobjective']").text($(this).attr("ows_LabelProblemStatementobjective"));
                    $("[id*='lblprojectmetrics']").text($(this).attr("ows_LabelProjectMetrics"));
                    $("[id*='lblarea']").text($(this).attr("ows_LabelArea"));
                    $("[id*='lblmetiric']").text($(this).attr("ows_LabelMetric"));
                    $("[id*='lblbaseline']").text($(this).attr("ows_LabelBaseLine"));
                    $("[id*='lblgoal']").text($(this).attr("ows_LabelGoal"));

                    $("[id*='lblcost']").text($(this).attr("ows_LabelCost"));
                    $("[id*='lblquality']").text($(this).attr("ows_LabelQuality"));
                    $("[id*='lbldelivery']").text($(this).attr("ows_LabelDelivery"));
                    $("[id*='lblother']").text($(this).attr("ows_LabelOther"));
                    $("[id*='lblother1']").text($(this).attr("ows_LabelOther"));


                    $("[id*='lblbenefits']").text($(this).attr("ows_LabelBenefits"));
                    $("[id*='lblProjectCosts']").text($(this).attr("ows_LabelProjectCosts"));
                    $("[id*='lblfinancialanalysis']").text($(this).attr("ows_LabelFinancialAnalysis"));

                    $("[id*='lblEcost']").text($(this).attr("ows_LabelEcost"));
                    $("[id*='lblRealAB']").text($(this).attr("ows_LabelRealAnnualizedBenefits"));
                    $("[id*='lblRealCost']").text($(this).attr("ows_LabelRealCost"));
                    $("[id*='lblmilestones']").text($(this).attr("ows_LabelMilestones"));

                    $("[id*='lblProjectAuthorization']").text($(this).attr("ows_LabelProjectAuthorization"));
                    $("[id*='lbldefine']").text($(this).attr("ows_LabelDefine"));
                    $("[id*='labelmeasure']").text($(this).attr("ows_LabelMeasure"));
                    $("[id*='lblanalyze']").text($(this).attr("ows_LabelAnalyze"));
                    $("[id*='labelimprove']").text($(this).attr("ows_LabelImprove"));
                    $("[id*='labelcontrol']").text($(this).attr("ows_LabelControl"));
                    $("[id*='lblfinalcontrol']").text($(this).attr("ows_LabelFinalReport"));

                    $("[id*='lblProjectTeam']").text($(this).attr("ows_LabelProjectTeam"));


                    var GridProjectTeam = document.getElementById("<%=GridProjectTeam.ClientID %>");
                    if (GridProjectTeam != null && GridProjectTeam.rows.length > 0) {
                        GridProjectTeam.rows[0].cells[0].title = $(this).attr("ows_GridTeamMember");
                        GridProjectTeam.rows[0].cells[0].innerHTML = $(this).attr("ows_GridTeamMember");

                        GridProjectTeam.rows[0].cells[2].title = $(this).attr("ows_GridRole");
                        GridProjectTeam.rows[0].cells[2].innerHTML = $(this).attr("ows_GridRole");

                        GridProjectTeam.rows[0].cells[1].title = $(this).attr("ows_GridDepartment");
                        GridProjectTeam.rows[0].cells[1].innerHTML = $(this).attr("ows_GridDepartment");

                    }

                    $("[id*='lblProjectUpdates']").text($(this).attr("ows_LabelProjectUpdates"));
                    $("[id*='lblchartsQuad']").text($(this).attr("ows_LabelQuadCharts"));
                    $("[id*='lblFeedProject']").text($(this).attr("ows_LabelProjectFeed"));

                    var GridQuadDetails = document.getElementById("<%=GridQuadDetails.ClientID %>");
                    if (GridQuadDetails != null && GridQuadDetails.rows.length > 0) {
                        GridQuadDetails.rows[0].cells[0].title = $(this).attr("ows_GridQuadateuploaded");
                        GridQuadDetails.rows[0].cells[0].innerHTML = $(this).attr("ows_GridQuadateuploaded");

                        GridQuadDetails.rows[0].cells[1].title = $(this).attr("ows_GridQuadBy");
                        GridQuadDetails.rows[0].cells[1].innerHTML = $(this).attr("ows_GridQuadBy");

                        GridQuadDetails.rows[0].cells[2].title = $(this).attr("ows_GridQuadFile");
                        GridQuadDetails.rows[0].cells[2].innerHTML = $(this).attr("ows_GridQuadFile");

                    }


                    $("[id*='lblDiscnotes']").text($(this).attr("ows_LabelDiscussionNotes"));

                    $("[id*='lnkBackgroundAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkBackgroundRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkBenefitesAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkBenefitesRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkcontrolAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkcontrolRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkCostAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkCostRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkFinancialAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkFinancialAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkFinancialRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkMileStonesAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkMileStonesRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkprojectmetricsAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkprojectmetricsRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkprbStatementAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkprbStatementRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkDefineAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkDefineRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkMeasureAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkMeasureRemobveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkAnalyzeAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkAnalyzeRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkInvestigateAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkInvestigateRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkcontrolAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkcontrolRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));
                    $("[id*='lnkfinalReportAddDocuments']").text($(this).attr("ows_LabelLinkAdd"));
                    $("[id*='lnkfinalReportRemoveDocuments']").text($(this).attr("ows_LabelLinkRemove"));


                    $("[id*='lblgates']").text($(this).attr("ows_LabelGates"));
                    $("[id*='lblgates1']").text($(this).attr("ows_LabelGates"));
                    $("[id*='lblcommebts']").text($(this).attr("ows_LabelComments"));
                    $("[id*='lblcompletiondetails']").text($(this).attr("ows_LabelCompleDetails"));

                    $("[id*='lblotherattachment']").text($(this).attr("ows_LabelotherAttachment"));
                    $("[id*='lbluploadFiles']").text($(this).attr("ows_LabelUploadFiles"));

                    $("[id*='lblfromothersite']").text($(this).attr("ows_LabelLinkfromothersite"));



                    var GridAttachmentSecond = document.getElementById("<%=GridAttachmentSecond.ClientID %>");
                    if (GridAttachmentSecond != null && GridAttachmentSecond.rows.length > 0) {
                        GridAttachmentSecond.rows[0].cells[0].title = $(this).attr("ows_GridlinkName");
                        GridAttachmentSecond.rows[0].cells[0].innerHTML = $(this).attr("ows_GridlinkName");

                        GridAttachmentSecond.rows[0].cells[1].title = $(this).attr("ows_Gridlinkurl");
                        GridAttachmentSecond.rows[0].cells[1].innerHTML = $(this).attr("ows_Gridlinkurl");



                    }

                    $("[id*='lblapprovallogs']").text($(this).attr("ows_LabelApprovalLogs"));
                    $("[id*='lblworkflowlogs']").text($(this).attr("ows_LabelWorkflowlogs"));
                    $("[id*='lblactionlogs']").text($(this).attr("ows_LabelActionLogs"));


                    var GridviewAction = document.getElementById("<%=grdviewAction.ClientID %>");
                    if (GridviewAction != null && GridviewAction.rows.length > 0) {
                        GridviewAction.rows[0].cells[0].title = $(this).attr("ows_GridAction");
                        GridviewAction.rows[0].cells[0].innerHTML = $(this).attr("ows_GridAction");

                        GridviewAction.rows[0].cells[1].title = $(this).attr("ows_GridActionBy");
                        GridviewAction.rows[0].cells[1].innerHTML = $(this).attr("ows_GridActionBy");

                        GridviewAction.rows[0].cells[2].title = $(this).attr("ows_GridActionDate");
                        GridviewAction.rows[0].cells[2].innerHTML = $(this).attr("ows_GridActionDate");

                        GridviewAction.rows[0].cells[3].title = $(this).attr("ows_GridActionComments");
                        GridviewAction.rows[0].cells[3].innerHTML = $(this).attr("ows_GridActionComments");

                    }

                    $("[id*='lblPlannedCost']").text($(this).attr("ows_LblPlannedCost"));
                    $("[id*='lblPlannedBenefit']").text($(this).attr("ows_LblPlannedBenefit"));
                    $("[id*='lblActualCost']").text($(this).attr("ows_LblActualCost"));
                    $("[id*='lblActualBenefit']").text($(this).attr("ows_LblActualBenefit"));

                    $("[id*='btnSixSigmaSave']").val($(this).attr("ows_BtnSave"));
                    $("[id*='btnProjectAuthorization']").val($(this).attr("ows_BtnProjectAuthorization"));
                    $("[id*='btnSixSigmaClose']").val($(this).attr("ows_BtnClose"));
                    $("[id*='btnAddTeamMember']").val($(this).attr("ows_BtnAdd"));
                    $("[id*='btnAddAttachment']").val($(this).attr("ows_BtnAdd"));
                    $("[id*='btnNotify']").val($(this).attr("ows_BtnNotify"));
                    $("[id*='btnSponsorApproval']").val($(this).attr("ows_BtnSponserApproval"));
                    $("[id*='btnReturnProjectLead']").val($(this).attr("ows_BtnReturntoPrjLead"));
                    $("[id*='btnBBApproval']").val($(this).attr("ows_BtnBBApproval"));
                    $("[id*='btnAddQuadCharts']").val($(this).attr("ows_BtnAdd"));
                    $("[id*='btnMakeCopy']").val($(this).attr("ows_Btncopy"));
                    $("[id*='AddReply']").val($(this).attr("ows_BtnPost"));
                    $("[id*='btnDefineRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnDefineBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnDefineReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnMeasureRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnMeasureBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnMeasureReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnAnalyzeRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnAnalyzeBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnAnalyzeReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnInvestigateRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnInvestigateBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnInvestigateReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnControlRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnControlBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnControlReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnFinalreportRequestApproval']").val($(this).attr("ows_BtnRequestApproval"));
                    $("[id*='btnFinalreportBBApproval']").val($(this).attr("ows_BtnGateBBApproval"));
                    $("[id*='btnFinalreportReturntoProjectlead']").val($(this).attr("ows_BtnGateReturntoPL"));
                    $("[id*='btnupload']").val($(this).attr("ows_Btnupload"));
                    $("[id*='btnreassignBlackbeltuser']").val($(this).attr("ows_BtnReassign"));
                    $("[id*='btnreassignGreenbeltuser']").val($(this).attr("ows_BtnReassign"));
                });
            }
        });
    }
</script>
<script type="text/javascript">
    function portal_modalDialogClosedCallback(result, returnValue) {

        if (result === SP.UI.DialogResult.OK) {

            document.getElementById('<%=hdnGetPopupReturnValue.ClientID%>').value = returnValue;


            var DraftValue = document.getElementById("<%=lnkResolve.ClientID%>").innerText;
            if (DraftValue == "") {
                SetPickerValue('<%=hdnGetPopupReturnValue.ClientID%>', returnValue, "");
                document.getElementById('<%=lnkResolve.ClientID%>').click();
            }

        }
        if (result === SP.UI.DialogResult.cancel) {
            //alert("Cancel was clicked!");
        }
    }

    function closecallbackAttach(result, value) {
        // Check if OK button was clicked.
        if (result === SP.UI.DialogResult.OK) {
            // Set the value of the hidden textbox on the web part 
            // with the value passed by the OK button event.
            var ispostback = setValue('.webparthiddenfield', value);
            if (ispostback == true) {
                // Postback the page so the web part lifecycle is reinitiated.
                //postpage();
                document.getElementById('<%=lnkResolve.ClientID%>').click();
            }
        }
    }

    function closecallbackAttachment(result, value) {
        // Check if OK button was clicked.
        if ((result === SP.UI.DialogResult.OK) || (result === SP.UI.DialogResult.cancel)) {
            // Set the value of the hidden textbox on the web part 
            // with the value passed by the OK button event.
            var ispostback = setValue('.webparthiddenfield', value);
            if (ispostback == true) {
                // Postback the page so the web part lifecycle is reinitiated.
                //postpage();
                document.getElementById('<%=lnkbtnupload.ClientID%>').click();
            }
        }
    }
    function SetPickerValue(pickerid, key, dispval) {
        //alert(key);
        document.getElementById('<%=Hidden1.ClientID%>').value = key;

        var user = key;
        var counter = 0;

        $('div[title="People Picker"]').each(function () {
            //alert(counter);
            if (counter == 1) { // this is because I was setting the 2nd people picker on the page
                $(this).html(" ");
                $(this).html(user);  // <-- this is the magic
            }
            counter++;



            // $('a[title="Check Names"]').click();  // <-- clicks the "Check Name" button programmatically to resolve your user
        });




    }
    function portal_modalDialogClosedCallback1(result, returnValue) {

        if (result === SP.UI.DialogResult.OK) {

            document.getElementById('<%=hdnGetPopupReturnValue1.ClientID%>').value = returnValue;
            document.getElementById('<%=lnkResolve1.ClientID%>').click();

            var DraftValue = document.getElementById("<%=lnkResolve2.ClientID%>").innerText;
            if (DraftValue == "") {
                SetPickerValue1('<%=hdnGetPopupReturnValue1.ClientID%>', returnValue, "");
                document.getElementById('<%=lnkResolve2.ClientID%>').click();
            }





        }
        if (result === SP.UI.DialogResult.cancel) {
            //alert("Cancel was clicked!");
        }
    }
    function SetPickerValue1(pickerid, key, dispval) {
        //alert(key);
        document.getElementById('<%=Hidden1.ClientID%>').value = key;

        var user = key;
        var counter = 0;



        $('div[title="People Picker"]').each(function () {
            //alert(counter);
            if (counter == 2) { // this is because I was setting the 2nd people picker on the page
                $(this).html(" ");
                $(this).html(user);  // <-- this is the magic
            }
            counter++;



            // $('a[title="Check Names"]').click();  // <-- clicks the "Check Name" button programmatically to resolve your user
        });




    }


    function uploadValidation(control) {
        var row = document.getElementById('<%=trUploadErrorMsg.ClientID%>');
        var uploadErrorMsg = document.getElementById('<%=lblUploadErrorMsg.ClientID%>');
        var fullPath = document.getElementById('<%=UploadFile.ClientID%>').value;

        if (fullPath == "") {
            row.style.display = "";
            uploadErrorMsg.style.display = "inherit";
            uploadErrorMsg.innerHTML = "Please choose a file which you want to upload.";
            return false;
        }
        var file = fullPath.split(/(\\|\/)/g).pop();
        var filename = file.substring(0, file.lastIndexOf('.'));
        var fileExtnsn = file.substring(file.lastIndexOf('.'));


        if (fileExtnsn.toLowerCase() == ".htm" || fileExtnsn.toLowerCase() == ".html") {
            row.style.display = "";
            uploadErrorMsg.style.display = "inherit";
            uploadErrorMsg.innerHTML = "Files with .htm or .html extension cannot be uploaded to this site.";
            return false;
        }

        FreezeScreen();
    }


    function FreezeScreen() {
        if (GlobalLanguageId == 1) {
            var temp = '<span>Please wait while processing.</span>';
            window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Loading...','" + temp + "',65,350);");
        }
        else if (GlobalLanguageId == 2) {
            var temp = '<span>Sil vous plaît patienter pendant le traitement.</span>';
            window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Chargement...','" + temp + "',65,350);");
        }

        var a = parent.document;
        $('.ms-dlgTitleBtns', a).attr('style', 'display: none');
        return true;
    }

    function unFreeze() {
        if (window.parent.waitDialog != null) {
            var a = parent.document;
            $('.ms-dlgTitleBtns', a).removeAttr('style');
            window.parent.waitDialog.close();
        }
        return true;
    }



    function isNumberKey1(evt) {
        var E2Value = document.getElementById("<%=txtplannedActualCost.ClientID%>").value;
        var E2Length = E2Value.length;
        var key;
        var keychar;
        if (window.event) key = window.event.keyCode;
        else if (e) key = e.which;
        else return true;
        keychar = String.fromCharCode(key);
        keychar = keychar.toLowerCase();
        if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27)) {
            return true;
        }
        else if ((("0123456789").indexOf(keychar) > -1)) {

            if (E2Length > 9) {
                if (GlobalLanguageId == 1) {
                    alert("Only  Numbers Allowed.");
                }
                else if (GlobalLanguageId == 2) {
                    alert("Seuls 10 numéros autorisés.");
                }

                return false;
            }


            return true;
        }
        else {
            return false;
        }
    }



    function isNumberKey2(evt) {
        var E2Value = document.getElementById("<%=txtplannedActualBenefits.ClientID%>").value;
        var E2Length = E2Value.length;
        var key;
        var keychar;
        if (window.event) key = window.event.keyCode;
        else if (e) key = e.which;
        else return true;
        keychar = String.fromCharCode(key);
        keychar = keychar.toLowerCase();
        if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27)) {
            return true;
        }
        else if ((("0123456789").indexOf(keychar) > -1)) {

            if (E2Length > 9) {
                if (GlobalLanguageId == 1) {
                    alert("Only  Numbers Allowed.");
                }
                else if (GlobalLanguageId == 2) {
                    alert("Seuls 10 numéros autorisés.");
                }
                return false;
            }


            return true;
        }
        else {
            return false;
        }
    }



    function isNumberKey3(evt) {
        var E2Value = document.getElementById("<%=txtActualCost.ClientID%>").value;
        var E2Length = E2Value.length;
        var key;
        var keychar;
        if (window.event) key = window.event.keyCode;
        else if (e) key = e.which;
        else return true;
        keychar = String.fromCharCode(key);
        keychar = keychar.toLowerCase();
        if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27)) {
            return true;
        }
        else if ((("0123456789").indexOf(keychar) > -1)) {

            if (E2Length > 9) {
                if (GlobalLanguageId == 1) {
                    alert("Only  Numbers Allowed.");
                }
                else if (GlobalLanguageId == 2) {
                    alert("Seuls 10 numéros autorisés.");
                }
                return false;
            }


            return true;
        }
        else {
            return false;
        }
    }



    function isNumberKey4(evt) {
        var E2Value = document.getElementById("<%=txtActualbenefits.ClientID%>").value;
        var E2Length = E2Value.length;
        var key;
        var keychar;
        if (window.event) key = window.event.keyCode;
        else if (e) key = e.which;
        else return true;
        keychar = String.fromCharCode(key);
        keychar = keychar.toLowerCase();
        if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27)) {
            return true;
        }
        else if ((("0123456789").indexOf(keychar) > -1)) {

            if (E2Length > 9) {
                if (GlobalLanguageId == 1) {
                    alert("Only  Numbers Allowed.");
                }
                else if (GlobalLanguageId == 2) {
                    alert("Seuls 10 numéros autorisés.");
                }
                return false;
            }


            return true;
        }
        else {
            return false;
        }
    }






</script>
<style type="text/css">
    a.lnktrashdelete {
        margin: 0px 0px 0px 0px;
        background: url(~/_layouts/15/PWC.Process.SixSigma/images/Trash.png) left no-repeat;
        padding: 0em 0em .4em 1.7em;
        text-decoration: none !important;
        font-weight: bold;
        letter-spacing: 0px;
    }

 
</style>
<asp:UpdatePanel ID="updatePanel" runat="server">
    <triggers>
        <asp:PostBackTrigger ControlID="btnupload" />
    </triggers>
    <contenttemplate>
        <center>
            <div id="tabs" style="width: 1040px;">
                <ul>
                    <li style="font-size: 10pt"><a id="tab1" href="#tabs-1"></a></li>
                    <li style="font-size: 10pt"><a id="tab2" href="#tabs-2"></a></li>
                    <li style="font-size: 10pt"><a id="tab3" href="#tabs-3"></a></li>
                    <li style="font-size: 10pt"><a id="tab4" href="#tabs-4"></a></li>
                    <li style="font-size: 10pt"><a id="tab5" href="#tabs-5"></a></li>
                </ul>
                <div id="tabs-1">
                    <table width="100%">
                        <tr style="height: 30px">
                            <td align="left" style="padding-top: 10px;">
                                <img alt="" src="/_layouts/15/PWC.Process.SixSigma/images/FormResource.jpg" />
                            </td>
                            <td runat="server" id="LocationControlId" align="right" width="5%">
                                <sharepoint:delegatecontrol runat="server" controlid="LocationControl" id="LocationControl" />
                            </td>
                        </tr>
                        <tr align="center" height="40px">
                            <td align="left" class="Form_Title">
                                <asp:Label ID="ProjectHeading" runat="server"></asp:Label>
                                <table>
                                    <tr>
                                        <td align="left" class="Form_Title_TechnicalData">
                                            <asp:Label ID="below" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" class="Project_Status">
                                <asp:Label ID="lblProjectStatus1" runat="server"></asp:Label>
                                <asp:Label ID="BasicInfoStatus" runat="server">Draft</asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Project_Language">
                                <asp:Label ID="lbllanguageid1" runat="server" Visible="true"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:DropDownList ID="ddlProcessForm1" Visible="true" runat="server" AutoPostBack="false"
                                    Width="150px" onchange="return GetLanguage(this);">
                                </asp:DropDownList>
                            </td>
                            <td class="Project_Id">
                                <asp:Label ID="lblTextProjectId1" runat="server"></asp:Label>
                                <asp:Label ID="lblProjectId1" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Form_Title">
                            </td>
                            <td align="right" class="Form_Title">
                                <asp:Label ID="lblProjectName1" runat="server"></asp:Label>
                                <asp:Label ID="ProjectName1" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="2">
                                <table cellpadding="2" cellspacing="2" width="100%">
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Heading" colspan="16">
                                            <asp:Label ID="lblprojectinformation" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Sub_Heading" colspan="16">
                                            <asp:Label ID="lblprojectidentification" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" width="20%" class="Project_Label">
                                            <asp:Label ID="lblnameproject" runat="server"></asp:Label><span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="12" width="30%" class="Project_Label_Control">
                                            <asp:TextBox ID="txtProjectName" Width="93%" runat="server" CssClass="Label_Control"></asp:TextBox>
                                        </td>
                                        <!--td colspan="2" class="Project_Label">
                                            <span>Project ID:</span> <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="2" class="Project_Label_Control">
                                            <asp:Label ID="lblProjectID" runat="server" CssClass="Label_Control"></asp:Label>
                                        </td-->
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblorganization" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="3" class="Project_Label_Control">
                                            <asp:DropDownList Width="210px" ID="ddlorgnisation" runat="server" CssClass="Label_Control">
                                            </asp:DropDownList>
                                        </td>
                                        <td colspan="4" class="Project_Label" width="20%">
                                            <asp:Label ID="lblprojectsponser" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="4" class="Project_Label_Control">
                                            <sharepoint:peopleeditor required="true" validationenabled="true" id="projectSponserUserEditor"
                                                runat="server" multiselect="false" visiblesuggestions="3" rows="1" allowmultipleentities="false"
                                                placebuttonsunderentityeditor="false" width="235px" autopostback="false" enableviewstate="false"
                                                selectionset="User" cssclass="Label_Control" />
                                        </td>
                                        <td bgcolor="#F1F1F1" align="left">
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblprojectplant" runat="server" Style="margin-left: -4px;"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="3" class="Project_Label_Control">
                                            <asp:DropDownList Width="210px" ID="ddlplant" runat="server" CssClass="Label_Control">
                                            </asp:DropDownList>
                                        </td>
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblblackbeltusers" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="4" class="Project_Label_Control">
                                            <sharepoint:peopleeditor required="true" validationenabled="true" id="BlackbeltuserEditor"
                                                runat="server" multiselect="false" visiblesuggestions="3" rows="1" enablebrowse="false"
                                                showbuttons="false" enabled="false" allowmultipleentities="false" placebuttonsunderentityeditor="false"
                                                width="235px" autopostback="false" enableviewstate="false" selectionset="User"
                                                sharepointgroup="BlackBelt" cssclass="Label_Control" />
                                        </td>
                                        <td bgcolor="#F1F1F1" align="center">
                                            <asp:UpdatePanel ID="BlackbeltuserEditorUpdatePanel" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnreassignBlackbeltuser" Height="30px" runat="server" Text="" Font-Names="Calibri"
                                                        Font-Size="12pt" OnClientClick="return OpenPopup('BB')" />
                                                    <asp:LinkButton ID="lnkResolve" runat="server" Text="" OnClick="lnkResolve_Click"
                                                        CssClass="ms-addnew"></asp:LinkButton>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblprojecttype" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="3" class="Project_Label_Control">
                                            <asp:DropDownList Width="210px" ID="ddlprojecttype" runat="server" CssClass="Label_Control">
                                            </asp:DropDownList>
                                        </td>
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblGreenbeltusers" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                        <td colspan="4" class="Project_Label_Control">
                                            <sharepoint:peopleeditor required="true" validationenabled="true" id="GreenbeltuserEditor"
                                                runat="server" multiselect="false" visiblesuggestions="3" rows="1" allowmultipleentities="false"
                                                showbuttons="false" enablebrowse="false" enabled="false" placebuttonsunderentityeditor="false"
                                                width="235px" autopostback="false" enableviewstate="false" selectionset="User"
                                                sharepointgroup="GreenBelt" cssclass="Label_Control" />
                                        </td>
                                        <td bgcolor="#F1F1F1" align="center">
                                            <asp:UpdatePanel ID="GreenbeltuserEditorUpdatePanel" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnreassignGreenbeltuser" Height="30px" runat="server" Text="" Font-Names="Calibri"
                                                        Font-Size="12pt" OnClientClick="return OpenPopup('GB')" />
                                                    <asp:LinkButton ID="lnkResolve2" runat="server" Text="" OnClick="lnkResolve2_Click"
                                                        CssClass="ms-addnew"></asp:LinkButton>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label">
                                            <asp:Label ID="lblTags" runat="server"></asp:Label>
                                        </td>
                                        <td colspan="12" class="Project_Label_Control">
                                            <taxonomy:taxonomywebtaggingcontrol runat="server" id="taxTags" visible="true" ismulti="true"
                                                cssclass="Label_Control"></taxonomy:taxonomywebtaggingcontrol>
                                        </td>
                                    </tr>
                                    <tr id="TrErrorLabel" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="ProjectIdentificationErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblBackgroud" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr id="TrDescription" style="height: 30px;">
                                        <td colspan="12" class="Project_Label_Control">
                                            <asp:TextBox ID="txtBackground" MaxLength="800" Rows="6" onkeydown="limitText(800);"
                                                onkeyup="limitText(800);" TextMode="MultiLine" Width="99%" Height="120px" runat="server"
                                                CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" width="26%" style="vertical-align: top;">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="backgrounddocs" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments1" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divBackground" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdBackgrounDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdBackgrounDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdBackgrounDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-Font-Size="10pt" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="BackgroundSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsId" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkBackgroundAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span id="spanadd" runat="server">/ </span>
                                                        <asp:LinkButton ID="lnkBackgroundRemoveDocuments" runat="server" Text="Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkBackgroundMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs1" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="BackgroudErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectBackGroundErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="12" bgcolor="#F1F1F1" align="right">
                                            <font size="1">(Maximum characters: 800)&nbsp;&nbsp;You have
                                                <input type="text" runat="server" id="txtCT1" size="3" value="800" readonly="readonly" />
                                                characters left.</font>
                                        </td>
                                        <td colspan="4" bgcolor="#F1F1F1" align="right">
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblproblemstatementobjective" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="12" class="Project_Label_Control">
                                            <asp:TextBox ID="txtProjectstatementobj" MaxLength="800" Rows="6" onkeydown="limitText(800);"
                                                onkeyup="limitText(800);" TextMode="MultiLine" Width="99%" Height="120px" runat="server"
                                                CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="vertical-align: top">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="backgrounddocs1" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments2" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments1" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divPrbStatement" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdprbStatmentDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdprbStatmentDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdprbStatmentDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="ProblemStatementSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsId1" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkprbStatementAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span id="spanadd1" runat="server">/ </span>
                                                        <asp:LinkButton ID="lnkprbStatementRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkProblemStatementMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs2" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="PSOErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectPSOErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="12" bgcolor="#F1F1F1" align="right">
                                            <font size="1">(Maximum characters: 800)&nbsp;&nbsp;You have
                                                <input type="text" runat="server" id="txtCT2" size="3" value="800" readonly="readonly" />
                                                characters left.</font>
                                        </td>
                                        <td colspan="4" bgcolor="#F1F1F1" align="right">
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblprojectmetrics" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="12" style="vertical-align: top">
                                            <table width="100%">
                                                <tr height="25px">
                                                    <td width="22%" id="Th1" class="Table_Header">
                                                        <asp:Label ID="lblarea" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="26%" id="Th2" class="Table_Header" align="center">
                                                        <asp:Label ID="lblmetiric" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="26%" id="Th3" class="Table_Header" align="center">
                                                        <asp:Label ID="lblbaseline" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="26%" id="Th4" class="Table_Header" align="center">
                                                        <asp:Label ID="lblgoal" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_HeaderDD" style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:DropDownList ID="ddlMetricCost" Width="130px" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtmetriccost" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtCostBaseline" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtCostGoal" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_HeaderDD" style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:DropDownList ID="ddlQualityMetrics" Width="130px" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtmetricquality" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtQualityBaseline" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtQualityGoal" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_HeaderDD" style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:DropDownList ID="ddlDeliveryMetrics" Width="130px" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtmetricdelivery" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtDeliveryBaseline" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtDeliveryGoal" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_HeaderDD" style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:DropDownList ID="ddlothermetric" Width="130px" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtmetricother" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtotherbaseline" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtothergoal" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_HeaderDD" style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:DropDownList ID="ddlothermetric1" Width="130px" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtmetricother1" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtotherbaseline1" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri';" align="center">
                                                        <asp:TextBox ID="txtothergoal1" Width="155px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4" align="center" style="padding-right: 5px; font-size: 12pt; font-family: 'swis721 LtCn BT';
                                            vertical-align: top">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="projetmetrics" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments3" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments123" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divPrjectMetrics" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdprjectMetricsDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdprjectMetricsDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdprjectMetricsDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="ProjectMetricsSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsI123" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkprojectmetricsAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span style="color: #0071c6; font-size: 10pt; font-weight: bold" id="spanadd123"
                                                            runat="server">/ </span>
                                                        <asp:LinkButton ID="lnkprojectmetricsRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkProjectMetricsMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs3" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="ProjectMetricsErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectMetrErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblbenefits" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label_Control" valign="top">
                                            <table width="100%">
                                                <tr height="25px">
                                                    <td valign="top" class="Table_Header">
                                                        <asp:Label ID="lblPlannedCost" runat="server">Planned Cost ($)</asp:Label>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td>
                                                        <asp:TextBox ID="txtplannedActualCost" onkeypress="return isNumberKey1(event)" Width="98%"
                                                            runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr height="15px">
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td valign="top" class="Table_Header">
                                                        <asp:Label ID="lblPlannedBenefit" runat="server">Planned Benefits ($)</asp:Label>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td>
                                                        <asp:TextBox ID="txtplannedActualBenefits" onkeypress="return isNumberKey2(event)"
                                                            Width="98%" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" class="Project_Label_Control">
                                            <asp:TextBox ID="txtBenefits" MaxLength="800" Rows="6" onkeydown="limitText(800);"
                                                onkeyup="limitText(800);" TextMode="MultiLine" Width="99%" Height="120px" runat="server"
                                                CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="vertical-align: top;">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="backgrounddocs2" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments4" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments2" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divBenifits" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdBenfitsDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdBenfitsDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdBenfitsDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="BenfitsSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsId2" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkBenefitesAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span id="spanadd2" runat="server">/ </span>
                                                        <asp:LinkButton ID="lnkBenefitesRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkBenfitsMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs4" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="BenefitsErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectBenefitsErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="12" bgcolor="#F1F1F1" align="right">
                                            <font size="1">(Maximum characters: 800)&nbsp;&nbsp;You have
                                                <input type="text" runat="server" id="txtCT3" size="3" value="800" readonly="readonly" />
                                                characters left.</font>
                                        </td>
                                        <td colspan="4" bgcolor="#F1F1F1" align="right">
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblProjectCosts" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td colspan="4" class="Project_Label_Control" valign="top">
                                            <table width="100%">
                                                <tr height="25px">
                                                    <td valign="top" class="Table_Header">
                                                        <asp:Label ID="lblActualCost" runat="server">Actual Cost ($)</asp:Label>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td>
                                                        <asp:TextBox ID="txtActualCost" Width="98%" onkeypress="return isNumberKey3(event)"
                                                            runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr height="15px">
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td valign="top" class="Table_Header">
                                                        <asp:Label ID="lblActualBenefit" runat="server">Actual Benefits ($)</asp:Label>
                                                    </td>
                                                </tr>
                                                <tr height="25px">
                                                    <td>
                                                        <asp:TextBox ID="txtActualbenefits" Width="98%" onkeypress="return isNumberKey4(event)"
                                                            runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" class="Project_Label_Control">
                                            <asp:TextBox ID="txtcosts" MaxLength="800" Rows="6" onkeydown="limitText(800);" onkeyup="limitText(800);"
                                                TextMode="MultiLine" Width="99%" Height="120px" runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" align="center" style="padding-right: 5px; font-size: 12pt; font-family: 'swis721 LtCn BT';
                                            vertical-align: top">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="backgrounddocs3" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments5" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments3" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divCosts" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdCostsDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdCostsDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdCostsDocuments_DataBound" OnPageIndexChanging="OnPageIndexChangeDocuments"
                                                                Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="CosstsSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsId3" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkCostAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span style="color: #0071c6; font-size: 10pt; font-weight: bold" id="spanadd3" runat="server">
                                                            / </span>
                                                        <asp:LinkButton ID="lnkCostRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkCosstsMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs5" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="CostsErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectCostsErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="12" bgcolor="#F1F1F1" align="right">
                                            <font size="1">(Maximum characters: 800)&nbsp;&nbsp;You have
                                                <input type="text" runat="server" id="txtCT4" size="3" value="800" readonly="readonly" />
                                                characters left.</font>
                                        </td>
                                        <td colspan="4" bgcolor="#F1F1F1" align="right">
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px; display: none">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblfinancialanalysis" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px; display: none">
                                        <td colspan="12" style="vertical-align: top">
                                            <table width="100%">
                                                <tr height="25px">
                                                </tr>
                                                <tr height="25px">
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4" align="center" style="padding-right: 5px; font-size: 12pt; font-family: 'swis721 LtCn BT';
                                            vertical-align: top">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="Td2" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments6" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrFinancialdocuments" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divfinancial" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdFinancialDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdFinancialDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdFinancialDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="FinancialSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddFinancialDocuments" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkFinancialAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span style="color: #0071c6; font-size: 10pt; font-weight: bold" id="span4" runat="server">
                                                            / </span>
                                                        <asp:LinkButton ID="lnkFinancialRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkFinancialMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs6" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="FinanceErrorLabel" runat="server" style="display: none">
                                        <td colspan="12" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectFinanceErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblmilestones" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr id="TrBenefits" align="center" style="height: 30px;">
                                        <td colspan="12" bgcolor="#F1F1F1" valign="top">
                                            <table width="100%">
                                                <tr>
                                                    <td width="9%" class="Table_Header">
                                                    </td>
                                                    <td width="13%" id="ProLatestApproved" class="Table_Header">
                                                        <asp:Label ID="lblProjectAuthorization" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="ForLatestForeCasted" class="Table_Header">
                                                        <asp:Label ID="lbldefine" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="ActLatestActual" class="Table_Header">
                                                        <asp:Label ID="labelmeasure" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="VarLatestVariance" class="Table_Header">
                                                        <asp:Label ID="lblanalyze" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="Improve" class="Table_Header">
                                                        <asp:Label ID="labelimprove" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="Control" class="Table_Header">
                                                        <asp:Label ID="labelcontrol" runat="server"></asp:Label>
                                                    </td>
                                                    <td width="13%" id="FinalReportApproved" class="Table_Header">
                                                        <asp:Label ID="lblfinalcontrol" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_Header">
                                                        <span id="CostLatest">Approved Plan</span>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateProjectAuthorization" autopostback="true"
                                                            ondatechanged="Plandatechange" runat="server" isrequiredfield="False" dateonly="true"
                                                            cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateDefine" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateMeasure" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateAnalyze" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateImprove" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateControl" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="PlandateFinalReportApprove" autopostback="true" ondatechanged="Plandatechange"
                                                            runat="server" isrequiredfield="False" dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Table_Header">
                                                        <span id="stdate">Actual</span>
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateProjectAuthorization" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateDefine" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateMeasure" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateAnalyze" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateImprove" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateControl" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                    <td style="font-size: 10pt; font-family: 'Calibri'; text-align='left';">
                                                        <sharepoint:datetimecontrol id="ActualdateFinalReportApprove" runat="server" isrequiredfield="False"
                                                            dateonly="true" cssclasstextbox="dateClass" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4" align="center" style="padding-right: 5px; font-size: 12pt; font-family: 'swis721 LtCn BT';
                                            vertical-align: top">
                                            <table width="100%" cellspacing="0">
                                                <tr class="ms-WPHeader">
                                                    <td id="backgrounddocs4" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblsupportingdocuments7" runat="server"></asp:Label>
                                                        </h3>
                                                    </td>
                                                </tr>
                                                <tr id="TrGridDocuments4" runat="server">
                                                    <td>
                                                        <div style="width: 100%; overflow: auto;" id="divMilestones" runat="server">
                                                            <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdMilestonesDocuments" GridLines="None"
                                                                CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdMilestonesDocuments_RowDataBound"
                                                                AllowPaging="false" PageSize="5" OnDataBound="grdMilestonesDocuments_DataBound"
                                                                OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                                runat="server" OnClientClick="SetTab(0)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                                        ItemStyle-VerticalAlign="Top">
                                                                        <ItemTemplate>
                                                                            <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                        ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                            <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                                </Columns>
                                                                <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                                    Font-Size="10pt" CssClass="DiscussionPager" />
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="MilestonesSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="AddDocumentsId4" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkMileStonesAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span id="spanadd4" runat="server">/ </span>
                                                        <asp:LinkButton ID="lnkMileStonesRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkMilestonesMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs7" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="MilestonesErrorLabel" runat="server" style="display: none">
                                        <td colspan="16" bgcolor="#F1F1F1" align="left">
                                            <asp:Label ID="ProjectMilestonesErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Sub_Heading">
                                            <asp:Label ID="lblProjectTeam" runat="server"></asp:Label>
                                            <span class="Label_Mandatory">*</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="16" align="left" bgcolor="#F1F1F1" valign="top">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="lnkResolve1" runat="server" Text="" OnClick="lnkResolve1_Click"
                                                            CssClass="ms-addnew"></asp:LinkButton>
                                                        <asp:GridView ID="GridProjectTeam" ShowFooter="true" DataKeyNames="IDProjectTeam"
                                                            AllowPaging="false" PageSize="5" Width="100%" CssClass="myGridStylePeople" runat="server"
                                                            RowStyle-Wrap="false" AutoGenerateColumns="false" BorderStyle="None" EditRowStyle-BorderStyle="None"
                                                            OnRowCommand="GridProjectTeam_RowCommand" OnRowDataBound="GridProjectTeam_RowCommandDatabound"
                                                            OnPageIndexChanging="GridProjectTeam_PageIndexChanging" OnRowCancelingEdit="GridProjectTeam_RowCancelingEdit"
                                                            OnRowDeleting="GridProjectTeam_RowDeleting" OnRowEditing="GridProjectTeam_RowEditing"
                                                            OnRowUpdating="GridProjectTeam_RowUpdating" OnSelectedIndexChanged="GridProjectTeam_SelectedIndexChanged">
                                                            <AlternatingRowStyle />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTeamMember" Text="<%# Bind('TeamMember') %>" runat="server" Width="100px"></asp:Label>
                                                                        <asp:HiddenField ID="hdmMemberId" runat="server" Value="<%# Bind('MemberID') %>" />
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdnTeamMember" runat="server" Value="<%# Bind('TeamMember') %>" />
                                                                        <sharepoint:peopleeditor required="true" validationenabled="true" id="txtSPPeopleEditor"
                                                                            runat="server" multiselect="false" visiblesuggestions="3" rows="1" allowmultipleentities="false"
                                                                            placebuttonsunderentityeditor="false" width="200px" autopostback="false" enableviewstate="false"
                                                                            selectionset="User" />
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <sharepoint:peopleeditor required="true" validationenabled="true" id="txtFooterTeamMember"
                                                                            runat="server" multiselect="false" visiblesuggestions="3" rows="1" allowmultipleentities="false"
                                                                            placebuttonsunderentityeditor="false" width="200px" autopostback="false" enableviewstate="false"
                                                                            selectionset="User" />
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Department" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lbldepartment" Text="<%# Bind('Department') %>" runat="server" Width="100px"></asp:Label>
                                                                        <asp:HiddenField ID="hdndepartmentID" runat="server" Value="<%# Bind('Department') %>" />
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdndepartment" runat="server" Value="<%# Bind('Department') %>" />
                                                                        <asp:TextBox ID="txtdepartmentEditor" runat="server"></asp:TextBox>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:TextBox ID="txtfooterdepartmentEditor" runat="server"></asp:TextBox>
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTeamRole" Text="<%# Bind('TeamRole') %>" runat="server" Width="100px"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdnRoleType" runat="server" Value="<%# Bind('TeamRole') %>" />
                                                                        <asp:DropDownList ID="ddlTeamRoleEdit" Style="color: Black; font-size: 10pt; font-family: 'Calibri';"
                                                                            runat="server" Width="100px">
                                                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:DropDownList ID="ddlTeamRole" Style="color: Black; font-size: 10pt; font-family: 'Calibri';"
                                                                            runat="server" Width="100px">
                                                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="%" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblPercentage" Text="<%# Bind('Percentage') %>" runat="server" Width="100px"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdnPercentageRole" runat="server" Value="<%# Bind('Percentage') %>" />
                                                                        <asp:DropDownList ID="ddlEditPercentage" Style="color: Black; font-size: 10pt; font-family: 'Calibri';"
                                                                            runat="server" Width="100px">
                                                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                                                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                                                            <asp:ListItem Text="40" Value="40"></asp:ListItem>
                                                                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                                                            <asp:ListItem Text="60" Value="60"></asp:ListItem>
                                                                            <asp:ListItem Text="70" Value="70"></asp:ListItem>
                                                                            <asp:ListItem Text="80" Value="80"></asp:ListItem>
                                                                            <asp:ListItem Text="90" Value="90"></asp:ListItem>
                                                                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:DropDownList ID="ddlPercentage" Style="color: Black; font-size: 10pt; font-family: 'Calibri';"
                                                                            runat="server" Width="100px">
                                                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                                                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                                                            <asp:ListItem Text="40" Value="40"></asp:ListItem>
                                                                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                                                            <asp:ListItem Text="60" Value="60"></asp:ListItem>
                                                                            <asp:ListItem Text="70" Value="70"></asp:ListItem>
                                                                            <asp:ListItem Text="80" Value="80"></asp:ListItem>
                                                                            <asp:ListItem Text="90" Value="90"></asp:ListItem>
                                                                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-Width="10%">
                                                                    <EditItemTemplate>
                                                                        <asp:LinkButton ID="lbtnUpdate" runat="server" CommandName="Update" Text="Update" />
                                                                        <asp:LinkButton ID="lbtnCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                                    </EditItemTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="lbtnEditTeamMember" runat="server" CommandName="Edit" Text="Edit"
                                                                            ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif" OnClientClick="SetTab(0)" />&nbsp;&nbsp;&nbsp;
                                                                        <asp:ImageButton ID="lbtnDeleteTeamMember" runat="server" OnClientClick="return confirm('Are you sure you want to delete this record?')"
                                                                            Text="Delete" CommandName="Delete" CausesValidation="false" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/delete.png" />
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Button ID="btnAddTeamMember" runat="server" Text="Add" Width="60px" Font-Names="Calibri"
                                                                            Font-Size="10pt" OnClick="AddTeamMember" />
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnrowindex" Value="<%#((GridViewRow)Container).RowIndex %>"
                                                                            runat="server" />
                                                                        <asp:Button ID="btnNotify" Width="90px" Font-Names="Calibri" Font-Size="10pt" CommandName="Notify"
                                                                            CommandArgument="<%#((GridViewRow)Container).RowIndex %>" runat="server" Text="Notify" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnMailSent" runat="server" />
                                                                        <asp:Label ID="lblSentEmail" Visible="false" Text="<%# Bind('EmailSent') %>" ForeColor="Green"
                                                                            runat="server" Width="150px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#1D558F"></HeaderStyle>
                                                            <RowStyle Wrap="False"></RowStyle>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrErrorLabelProjectTeam" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="ProjectTeamErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="16" align="left" bgcolor="#F1F1F1" style="padding-left: 10px; padding-right: 10px"
                                            valign="top">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="tabs-2">
                    <table id="Table3" width="100%">
                        <tr style="height: 30px">
                            <td align="left" style="padding-top: 10px;">
                                <img alt="" src="/_layouts/15/PWC.Process.SixSigma/images/FormResource.jpg" />
                            </td>
                        </tr>
                        <tr align="center" height="40px">
                            <td align="left" class="Form_Title">
                                <asp:Label ID="ProjectHeading1" runat="server"></asp:Label>
                                <table>
                                    <tr>
                                        <td align="left" class="Form_Title_TechnicalData">
                                            <asp:Label ID="below1" runat="server">ABCCCC DATA</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" class="Project_Status">
                                <asp:Label ID="lblProjectStatus2" runat="server"></asp:Label>
                                <asp:Label ID="UpdatesStatus" runat="server">Draft</asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Project_Language">
                                <asp:Label ID="lbllanguageid2" runat="server" Visible="true"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:DropDownList ID="ddlProcessForm2" Visible="true" runat="server" AutoPostBack="false"
                                    Width="150px" onchange="return GetLanguage(this);">
                                </asp:DropDownList>
                            </td>
                            <td class="Project_Id">
                                <asp:Label ID="lblTextProjectId2" runat="server"></asp:Label>
                                <asp:Label ID="lblProjectId2" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Form_Title">
                            </td>
                            <td align="right" class="Form_Title">
                                <asp:Label ID="lblProjectName2" runat="server"></asp:Label>
                                <asp:Label ID="ProjectName2" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="2">
                                <table cellpadding="2" cellspacing="2" width="100%">
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Heading">
                                            <asp:Label ID="lblProjectUpdates" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td width="55%" colspan="8" class="Project_Sub_Heading">
                                            <asp:Label ID="lblchartsQuad" runat="server"></asp:Label>
                                        </td>
                                        <td width="45%" colspan="8" class="Project_Sub_Heading">
                                            <asp:Label ID="lblFeedProject" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td style="vertical-align: top" colspan="8" align="left">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="GridQuadDetails" ShowFooter="false" DataKeyNames="IDQuad" AllowPaging="false"
                                                            PageSize="5" Width="100%" CssClass="myGridStyleQuad" runat="server" RowStyle-Wrap="false"
                                                            AutoGenerateColumns="false" BorderStyle="None" EditRowStyle-BorderStyle="None"
                                                            OnPageIndexChanging="GridQuadDetails_PageIndexChanging" OnRowCommand="GridQuadDetails_RowCommand"
                                                            OnRowDataBound="GridQuadDetails_RowDataBound" OnRowDeleting="GridQuadDetails_RowDeleting"
                                                            OnRowEditing="GridQuadDetails_RowEditing" OnRowUpdating="GridQuadDetails_RowUpdating"
                                                            OnSelectedIndexChanged="GridQuadDetails_SelectedIndexChanged">
                                                            <AlternatingRowStyle />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblDateUploaded" Text="<%# Bind('DateUploaded') %>" runat="server"
                                                                            Width="50px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="40%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblBy" Text="<%# Bind('By') %>" runat="server" Width="100px"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center"
                                                                    ItemStyle-Width="30%">
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="lnkResolve1" runat="server" Text="<%# Bind('DocName') %>" NavigateUrl="<%# Bind('QuadChartFileUrl') %>"
                                                                            Target="_blank" CssClass="Grid_Link"></asp:HyperLink>
                                                                        <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-VerticalAlign="Top">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="QuadChartsEdit" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                            runat="server" OnClientClick="SetTab(1)" />
                                                                        <asp:ImageButton ID="lbtnDeleteQuadFiles" runat="server" OnClientClick="return confirm('Are you sure you want to delete this record?')"
                                                                            Text="Delete" CommandName="Delete" CausesValidation="false" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/delete.png" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#1D558F"></HeaderStyle>
                                                            <RowStyle Wrap="False"></RowStyle>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Button ID="btnAddQuadCharts" runat="server" Text="Add" Width="60px" Font-Names="Calibri"
                                                            Font-Size="10pt" OnClick="AddQuadCharts" />
                                                        <asp:Button ID="btnMakeCopy" runat="server" Text="Copy" Width="50px" Font-Names="Calibri"
                                                            Font-Size="10pt" OnClick="CopyLastItem" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="padding-left: 10px;" colspan="8" align="left">
                                            <table width="100%">
                                                <tr class="ms-WPHeader">
                                                    <td id="DiscussionTd" runat="server" class="ms-WPHeaderTd">
                                                        <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                            <asp:Label ID="lblDiscnotes" runat="server"></asp:Label></h3>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <table width="100%" cellspacing="0">
                                                            <tr>
                                                                <td width="100%" valign="top">
                                                                    <table id="pwcPplPickerTable" runat="server" align="left" style="width: 98%" cellspacing="0">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="TB_Reply" Font-Names="Arial, Helvetica, sans-serif" TextMode="MultiLine"
                                                                                    runat="server" Width="100%" Height="60px" Font-Size="8pt" CssClass="txtareaWidth"></asp:TextBox>
                                                                                <br />
                                                                            </td>
                                                                        </tr>
                                                                        <tr align="right">
                                                                            <td>
                                                                                <asp:Button ID="btnDeleteAllPost" Font-Names="Arial, Helvetica, sans-serif" runat="server"
                                                                                    OnClick="DeleteAllPost" Text="Delete All Post" Width="120px" Height="25px" Visible="false" />
                                                                                <asp:Button ID="Bt_AddReply" Font-Names="Arial, Helvetica, sans-serif" runat="server"
                                                                                    OnClick="Bt_AddReply_Click" Text="Post" Width="78px" Height="25px" Style="margin-right: -5px"
                                                                                    OnClientClick="FreezeScreen();" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr align="right">
                                                                            <td id="TddeletePost" runat="server" style="display: none">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <div style="width: 100%; overflow: auto" id="dvdiscussion" runat="server">
                                                                                    <asp:GridView ID="discussion" GridLines="None" runat="server" PageSize="5" OnRowDataBound="disRowDataBound"
                                                                                        AutoGenerateColumns="false" AllowPaging="false" OnRowCommand="discussion_RowCommand"
                                                                                        ShowHeader="false" Width="100%" CssClass="myGridStyle">
                                                                                        <Columns>
                                                                                            <asp:TemplateField ShowHeader="false" HeaderStyle-Font-Bold="true" ItemStyle-VerticalAlign="Top">
                                                                                                <ItemTemplate>
                                                                                                    <table width="100%" style="vertical-align: top">
                                                                                                        <tr>
                                                                                                            <td>
                                                                                                                <asp:Image runat="server" ID="ProfilePicture" ImageUrl="<%# Bind('ImageURL') %>"
                                                                                                                    Height="50" Width="50"></asp:Image>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <table width="100%">
                                                                                                                    <tr>
                                                                                                                        <td align="left">
                                                                                                                            <asp:Label ID="lblName" ForeColor="Black" Font-Bold="true" Font-Size="8pt" runat="server"
                                                                                                                                Text="<%# Bind('Author') %>" Style="text-align: left"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td align="right">
                                                                                                                            <asp:Label ID="lbldate" ForeColor="Black" runat="server" Font-Names="Arial, Helvetica, sans-serif"
                                                                                                                                Font-Size="8pt" Text="<%# Bind('Created') %>"></asp:Label>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td colspan="2" align="left">
                                                                                                                            <asp:Label ID="lblreply" ForeColor="Black" Font-Names="Arial, Helvetica, sans-serif"
                                                                                                                                Font-Size="8pt" Text="<%# Bind('reply') %>" Width="250px" runat="server" Style="text-align: left"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td align="right">
                                                                                                                            <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDiscussionId" />
                                                                                                                            <asp:LinkButton align="left" CommandName="Trash" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'
                                                                                                                                Style="font-size: 10pt; font-weight: bold; padding-left: -20px;" ID="lnktrashdelete"
                                                                                                                                runat="server" CssClass="lnktrashdelete"></asp:LinkButton>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </ItemTemplate>
                                                                                            </asp:TemplateField>
                                                                                        </Columns>
                                                                                        <PagerStyle CssClass="DiscussionPager" Width="100%" HorizontalAlign="Right" ForeColor="#0072bc"
                                                                                            Font-Bold="true" Font-Size="10pt" />
                                                                                    </asp:GridView>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="tabs-3">
                    <table id="Tab3" width="100%">
                        <tr style="height: 30px">
                            <td align="left" style="padding-top: 10px;">
                                <img alt="" src="/_layouts/15/PWC.Process.SixSigma/images/FormResource.jpg" />
                            </td>
                        </tr>
                        <tr align="center" height="40px">
                            <td align="left" class="Form_Title">
                                <asp:Label ID="ProjectHeading2" runat="server"></asp:Label>
                                <table>
                                    <tr>
                                        <td align="left" class="Form_Title_TechnicalData">
                                            <asp:Label ID="below2" runat="server">ABCCCC DATA</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" class="Project_Status">
                                <asp:Label ID="lblProjectStatus3" runat="server"></asp:Label>
                                <asp:Label ID="GatesStatus" runat="server">Draft</asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Project_Language">
                                <asp:Label ID="lbllanguageid3" runat="server" Visible="true"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:DropDownList ID="ddlProcessForm3" Visible="true" runat="server" AutoPostBack="false"
                                    Width="150px" onchange="return GetLanguage(this);">
                                </asp:DropDownList>
                            </td>
                            <td class="Project_Id">
                                <asp:Label ID="lblTextProjectId3" runat="server"></asp:Label>
                                <asp:Label ID="lblProjectId3" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Form_Title">
                            </td>
                            <td align="right" class="Form_Title">
                                <asp:Label ID="lblProjectName3" runat="server"></asp:Label>
                                <asp:Label ID="ProjectName3" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="2">
                                <table cellpadding="2" cellspacing="2" width="100%">
                                    <tr align="center" style="height: 40px;">
                                        <td colspan="16" class="Project_Heading">
                                            <asp:Label ID="lblgates" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr class="ms-WPHeader" align="center">
                                        <td colspan="4" id="gate" runat="server" class="ms-WPHeaderTd" width="25%">
                                            <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                <asp:Label ID="lblgates1" runat="server"></asp:Label>
                                        </td>
                                        <td colspan="8" id="comment" runat="server" class="ms-WPHeaderTd" width="50%">
                                            <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                <asp:Label ID="lblcommebts" runat="server"></asp:Label></h3>
                                        </td>
                                        <td colspan="4" id="supprtingdocs" runat="server" class="ms-WPHeaderTd" width="25%">
                                            <h3 class="ms-standardheader ms-WPTitle" style="text-align: justify; font-size: 20px !important;">
                                                <asp:Label ID="lblsupportingdocuments8" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="DefineColorId" runat="server" colspan="4" style="padding-left: 10px; font-family: Calibri;
                                            font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Define</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblDefineCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapproveddefineOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblonapprove" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="left" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtdefineComment" Rows="6" TextMode="MultiLine" Width="99%" Height="75px"
                                                runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top;" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivDefine" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdDefineDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdDefineDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdDefineDocuments_DataBound" OnPageIndexChanging="OnPageIndexChangeDocuments"
                                                    Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="DefineSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdDefine" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkDefineAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkDefineRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkDefinesMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs8" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrDefineError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="DefineErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px;" align="left" bgcolor="Black">
                                            <asp:Button ID="btnDefineRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnDefineRequestApproval')" />
                                            <asp:Button ID="btnDefineBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnDefineBBApproval')" />
                                            <asp:Button ID="btnDefineReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnDefineReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="MeasureColorId" runat="server" colspan="4" style="padding-left: 10px; font-family: Calibri;
                                            font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Measure</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblMeasureCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapprovedMeasureOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblonmeasure" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="center" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtMeasurecomment" Rows="6" TextMode="MultiLine" Width="99%" Height="75px"
                                                runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top;" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivMeasure" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdMeasureDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdMeasureDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdMeasureDocuments_DataBound"
                                                    OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="MeasureSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdMeasure" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkMeasureAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkMeasureRemobveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkMeasureMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs9" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrMeasureError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="MeasureErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px;" align="left" bgcolor="Black">
                                            <asp:Button ID="btnMeasureRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnMeasureRequestApproval')" />
                                            <asp:Button ID="btnMeasureBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnMeasureBBApproval')" />
                                            <asp:Button ID="btnMeasureReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnMeasureReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="AnalyzeColorId" runat="server" colspan="4" style="padding-left: 10px; font-family: Calibri;
                                            font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Analyze</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblAnalyzeCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapprovedAnalyzeOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblonanalyze" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="center" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtAnalyzecomment" Rows="6" TextMode="MultiLine" Width="99%" Height="75px"
                                                runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivAnalyze" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdAnalyzeDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdAnalyzeDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdAnalyzeDocuments_DataBound"
                                                    OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="AnalyzeSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdAnalyze" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkAnalyzeAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkAnalyzeRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkAnalyzeMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs10" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrAnalyzeError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="AnalyzeErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px;" align="left" bgcolor="Black">
                                            <asp:Button ID="btnAnalyzeRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnAnalyzeRequestApproval')" />
                                            <asp:Button ID="btnAnalyzeBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnAnalyzeBBApproval')" />
                                            <asp:Button ID="btnAnalyzeReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnAnalyzeReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="InvestigateColorId" runat="server" colspan="4" style="padding-left: 10px;
                                            font-family: Calibri; font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Improve</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblInvestigateCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapprovedImproveOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblonImprove" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="center" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtinvestigatecomment" Rows="6" TextMode="MultiLine" Width="99%"
                                                Height="75px" runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivInvestigate" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdInvestigateDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdInvestigateDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdInvestigateDocuments_DataBound"
                                                    OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="InvestigateSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdInvesigate" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkInvestigateAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkInvestigateRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkInvestigateMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs11" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrInvestigateError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="InvestigateErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px;" align="left" bgcolor="Black">
                                            <asp:Button ID="btnInvestigateRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnInvestigateRequestApproval')" />
                                            <asp:Button ID="btnInvestigateBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnInvestigateBBApproval')" />
                                            <asp:Button ID="btnInvestigateReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnInvestigateReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="ControlColorId" runat="server" colspan="4" style="padding-left: 10px; font-family: Calibri;
                                            font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Control</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblControlCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapprovedControlOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lbloncontrol" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="center" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtControlcomment" Rows="6" TextMode="MultiLine" Width="99%" Height="75px"
                                                runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivControl" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdControlDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdControlDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdControlDocuments_DataBound"
                                                    OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="ControlsSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdControls" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkcontrolAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkcontrolRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkControlMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs12" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrControlError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="ControlErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px;" align="left" bgcolor="Black">
                                            <asp:Button ID="btnControlRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnControlRequestApproval')" />
                                            <asp:Button ID="btnControlBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnControlBBApproval')" />
                                            <asp:Button ID="btnControlReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnControlReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td id="FinalReportColorId" runat="server" colspan="4" style="padding-left: 10px;
                                            font-family: Calibri; font-size: 12pt; font-weight: bold" align="center" bgcolor="#F1F1F1">
                                            <span>Final Report</span>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblFinalReportCompletionDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblapprovedFinalReportOn" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="lblonfinal" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="8" style="font-size: 12pt; font-family: 'Calibri';" align="center" bgcolor="#F1F1F1">
                                            <asp:TextBox ID="txtFinalreportcomment" Rows="6" TextMode="MultiLine" Width="99%"
                                                Height="75px" runat="server" CssClass="TextBoxCSS"></asp:TextBox>
                                        </td>
                                        <td colspan="4" style="padding-left: 10px; vertical-align: top" align="left">
                                            <div style="width: 100%; overflow: auto;" id="DivFinalReport" runat="server">
                                                <asp:GridView RowStyle-HorizontalAlign="Left" ID="grdFinalReportDocuments" GridLines="None"
                                                    CssClass="myGridStyle" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdFinalReportDocuments_RowDataBound"
                                                    AllowPaging="false" PageSize="5" OnDataBound="grdFinalReportDocuments_DataBound"
                                                    OnPageIndexChanging="OnPageIndexChangeDocuments" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="DocumentsimageFaculty" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif"
                                                                    runat="server" OnClientClick="SetTab(2)" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="5%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imageType" ImageUrl="" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="25%" HeaderStyle-CssClass="Attachment_Grid_Title" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lblName" ForeColor="Black" Text="<%# Bind('Name') %>" runat="server"></asp:HyperLink>
                                                                <asp:HiddenField runat="server" Value='<%# Bind("ID") %>' ID="hdnDocumentsID" />
                                                                <asp:HiddenField runat="server" Value='<%# Bind("Url") %>' ID="hdnUrl" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Create Personal Copy" HeaderStyle-Wrap="false"  
                                                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%" HeaderStyle-CssClass="Attachment_Grid_Title"
                                                                            HeaderStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate> --%>
                                                        <%-- Hidden field hdnDocumentsID & hdnUrl copied in above template field -- By Sanjala 4Dec15 --%>
                                                        <%--
                                                                                <asp:Image ID="imageFaculty" ImageUrl="~/_layouts/PWC.MeetingSpace/Images/COPY.gif"
                                                                                    runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>--%>
                                                        <asp:CommandField ShowSelectButton="True" ItemStyle-CssClass="HiddenColumn" HeaderStyle-CssClass="HiddenColumn" />
                                                    </Columns>
                                                    <PagerStyle Width="100%" HorizontalAlign="Center" ForeColor="#0072bc" Font-Bold="true"
                                                        Font-Size="10pt" CssClass="DiscussionPager" />
                                                </asp:GridView>
                                            </div>
                                            <table width="100%">
                                                <tr id="FinalReportSupportingDocuments" runat="server" style="display: none;">
                                                    <td id="TdFinalReprt" runat="server" class="Link_Button">
                                                        <asp:LinkButton ID="lnkfinalReportAddDocuments" runat="server" Text="Add"></asp:LinkButton>
                                                        <span>/ </span>
                                                        <asp:LinkButton ID="lnkfinalReportRemoveDocuments" runat="server" Text=" Remove"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr id="AddLinkFinalReportMsg" runat="server" style="display: none; height: 85px;">
                                                    <td class="Link_Error_Label">
                                                        <asp:Label ID="lblsavesupportingdocs13" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="TrFinalReportError" runat="server" style="display: none">
                                        <td colspan="16" class="Project_Error_Label">
                                            <asp:Label ID="FinalReportErrorLabel" runat="server" CssClass="Project_Error_Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 30px;">
                                        <td align="center" colspan="16" style="padding-left: 10px; width: 15%;" align="left"
                                            bgcolor="Black">
                                            <asp:Button ID="btnFinalreportRequestApproval" runat="server" Text="Request BB Approval"
                                                Height="30px" Width="280px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnFinalreportRequestApproval')" />
                                            <asp:Button ID="btnFinalreportBBApproval" runat="server" Text="BB Approval" Height="30px"
                                                Width="200px" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnFinalreportBBApproval')" />
                                            <asp:Button ID="btnFinalreportReturntoProjectlead" runat="server" Width="180px" Height="30px"
                                                Text="Return to Project Lead" Font-Names="Calibri" Font-Size="12pt" OnClientClick="return ProjectComments('btnFinalreportReturntoProjectlead')" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="tabs-4">
                    <table id="Table7" width="100%">
                        <tr style="height: 30px">
                            <td align="left" style="padding-top: 10px;">
                                <img alt="" src="/_layouts/15/PWC.Process.SixSigma/images/FormResource.jpg" />
                            </td>
                        </tr>
                        <tr align="center" height="40px">
                            <td align="left" class="Form_Title">
                                <asp:Label ID="ProjectHeading3" runat="server"></asp:Label>
                                <table>
                                    <tr>
                                        <td align="left" class="Form_Title_TechnicalData">
                                            <asp:Label ID="below3" runat="server">ABCCCC DATA</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" class="Project_Status">
                                <asp:Label ID="lblProjectStatus4" runat="server"></asp:Label>
                                <asp:Label ID="attachmentStatus" runat="server">Draft</asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Project_Language">
                                <asp:Label ID="lbllanguageid4" runat="server" Visible="true"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:DropDownList ID="ddlProcessForm4" Visible="true" runat="server" AutoPostBack="false"
                                    Width="150px" onchange="return GetLanguage(this);">
                                </asp:DropDownList>
                            </td>
                            <td class="Project_Id">
                                <asp:Label ID="lblTextProjectId4" runat="server"></asp:Label>
                                <asp:Label ID="lblProjectId4" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Form_Title">
                            </td>
                            <td align="right" class="Form_Title">
                                <asp:Label ID="lblProjectName4" runat="server"></asp:Label>
                                <asp:Label ID="ProjectName4" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="2">
                                <table id="Table10" cellpadding="2" cellspacing="2" width="100%">
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Heading">
                                            <asp:Label ID="lblotherattachment" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Sub_Heading">
                                            <asp:Label ID="lbluploadFiles" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" id="tr4" runat="server" height="40px" valign="middle">
                                        <td style="padding-left: 5px;" bgcolor="#F1F1F1" align="left" valign="middle">
                                            <asp:FileUpload ID="UploadFile" runat="server" size="37" Height="27" Style="vertical-align: middle" />&nbsp;
                                            <asp:Button ID="btnupload" runat="server" Text="Upload" Font-Names="Calibri" Font-Size="12pt"
                                                OnClick="btnupload_Click" Style="vertical-align: middle" OnClientClick="return uploadValidation(this)" />
                                             <asp:LinkButton ID="lnkbtnupload" runat="server" Text="" OnClick="lnkResolve_Click"
                                                        CssClass="ms-addnew"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr id="trUploadErrorMsg" align="center" style="height: 30px; display: none;" runat="server">
                                        <td style="padding-left: 10px;" bgcolor="#E6E6E6" align="left">
                                            <asp:Label ID="lblUploadErrorMsg" runat="server" Font-Size="11pt" Font-Names="Calibri"
                                                Style="display: none;" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="Tr5" align="center" style="height: 30px;">
                                        <td bgcolor="#F1F1F1" align="left">
                                            <asp:GridView ID="AttachmentGrid" CssClass="myGridStyleActionLogs" Width="100%" runat="server"
                                                RowStyle-Wrap="false" HeaderStyle-BackColor="#1d558f" AutoGenerateColumns="false"
                                                OnRowDataBound="AttachmentGrid_RowDataBound" OnRowCommand="AttachmentGrid_RowCommand"
                                                OnPageIndexChanging="AttachmentGrid_PageIndexChanging" AllowPaging="True" OnPageIndexChanged="AttachmentGrid_PageIndexChanged"
                                                PageSize="5">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hypAttachmentLink" runat="server" Target="_blank" Text='<%# Eval("LinkFileName") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Left">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkvendorname" runat="server" CommandArgument='<%#Eval("ID")%>'
                                                                CommandName="Remove" Text='Remove' ForeColor="Red"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="" Visible="false" HeaderStyle-Wrap="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemPathHidden" Text="<%# Bind('ID') %>" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Sub_Heading">
                                            <asp:Label ID="lblfromothersite" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="Tr7" align="center" style="height: 30px;">
                                        <td bgcolor="#F1F1F1" align="left">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="GridAttachmentSecond" ShowFooter="true" DataKeyNames="IDAttachment"
                                                            AllowPaging="false" PageSize="5" Width="100%" CssClass="myGridStylePeople" runat="server"
                                                            RowStyle-Wrap="false" AutoGenerateColumns="false" BorderStyle="None" EditRowStyle-BorderStyle="None"
                                                            OnRowDataBound="GridAttachmentSecond_RowCommandDatabound" OnRowUpdating="GridAttachmentSecond_RowUpdating"
                                                            OnRowCancelingEdit="GridAttachmentSecond_RowCancelingEdit" OnRowDeleting="GridAttachmentSecond_RowDeleting"
                                                            OnRowEditing="GridAttachmentSecond_RowEditing">
                                                            <AlternatingRowStyle />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Link Name" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-Width="25%" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblLinkName" Text="<%# Bind('LinkName') %>" runat="server" Width="100px"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdnLinkName" runat="server" Value="<%# Bind('LinkName') %>" />
                                                                        <asp:TextBox ID="txtLinkName" runat="server"></asp:TextBox>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:TextBox ID="txtFooterLinkName" runat="server"></asp:TextBox>
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Link URL" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-Width="60%" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="HypAttachemntURL" runat="server" Text="<%# Bind('LinkURL') %>"
                                                                            NavigateUrl="<%# Bind('LinkURL') %>" Target="_blank" CssClass="Grid_Link"></asp:HyperLink>
                                                                        <asp:HiddenField runat="server" Value='<%# Bind("LinkURL") %>' ID="hdnLinkText" />
                                                                        <asp:Label ID="lblTeamRole" Text="<%# Bind('LinkURL') %>" Visible="false" runat="server"
                                                                            Width="100px"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:HiddenField ID="hdnLinkURL" runat="server" Value="<%# Bind('LinkURL') %>" />
                                                                        <asp:TextBox ID="txtLinkurlEdit" runat="server" Width="98%"></asp:TextBox>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:TextBox ID="txtFooterLinkURL" runat="server" Width="98%"></asp:TextBox>
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                                                    <EditItemTemplate>
                                                                        <asp:LinkButton ID="lbtnUpdate" runat="server" CommandName="Update" Text="Update" />
                                                                        <asp:LinkButton ID="lbtnCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                                    </EditItemTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="lbtnEditTeamMember" runat="server" CommandName="Edit" Text="Edit"
                                                                            ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/edititem.gif" OnClientClick="SetTab(3)" />&nbsp;&nbsp;&nbsp;
                                                                        <asp:ImageButton ID="lbtnDeleteTeamMember" runat="server" OnClientClick="return confirm('Are you sure you want to delete this record?')"
                                                                            Text="Delete" CommandName="Delete" CausesValidation="false" ImageUrl="~/_layouts/15/PWC.Process.SixSigma/images/delete.png" />
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Button ID="btnAddAttachment" runat="server" Text="Add" Width="60px" Font-Names="Calibri"
                                                                            Font-Size="10pt" OnClick="AddAttachmentNameURL" />
                                                                    </FooterTemplate>
                                                                    <FooterStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#1D558F"></HeaderStyle>
                                                            <RowStyle Wrap="False"></RowStyle>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="tabs-5">
                    <table id="Table8" width="100%">
                        <tr style="height: 30px">
                            <td align="left" style="padding-top: 10px;">
                                <img alt="" src="/_layouts/15/PWC.Process.SixSigma/images/FormResource.jpg" />
                            </td>
                        </tr>
                        <tr align="center" height="40px">
                            <td align="left" class="Form_Title">
                                <asp:Label ID="ProjectHeading4" runat="server"></asp:Label>
                                <table>
                                    <tr>
                                        <td align="left" class="Form_Title_TechnicalData">
                                            <asp:Label ID="below4" runat="server">ABCCCC DATA</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right" class="Project_Status">
                                <asp:Label ID="lblProjectStatus5" runat="server"></asp:Label>
                                <asp:Label ID="WorkflowStatus" runat="server">Draft</asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Project_Language">
                                <asp:Label ID="lbllanguageid5" runat="server" Visible="true" ForeColor="Black"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:DropDownList ID="ddlProcessForm5" Visible="true" runat="server" AutoPostBack="false"
                                    Width="150px" onchange="return GetLanguage(this);">
                                </asp:DropDownList>
                            </td>
                            <td class="Project_Id">
                                <asp:Label ID="lblTextProjectId5" runat="server"></asp:Label>
                                <asp:Label ID="lblProjectId5" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="left" class="Form_Title">
                            </td>
                            <td align="right" class="Form_Title">
                                <asp:Label ID="lblProjectName5" runat="server"></asp:Label>
                                <asp:Label ID="ProjectName5" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="2">
                                <table id="Table11" cellpadding="2" cellspacing="2" width="100%">
                                    <tr align="center" style="height: 40px;">
                                        <td class="Project_Heading">
                                            <asp:Label ID="lblapprovallogs" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td id="LogsTab" class="Project_Sub_Heading">
                                            <asp:Label ID="lblworkflowlogs" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="TrActionLogTableRIM" align="center" style="height: 40px;">
                                        <td colspan="16" align="left" bgcolor="#F1F1F1">
                                            <asp:GridView ID="GridApproval" CssClass="myGridStyleWorkflow" Width="100%" runat="server"
                                                RowStyle-Wrap="false" OnRowDataBound="GridApproval_RowDataBound" HeaderStyle-BackColor="#1D558F"
                                                AutoGenerateColumns="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td align="left" width="35%">
                                                                        <asp:Label ID="lblComments" Text="<%# Bind('Status') %>" runat="server"></asp:Label><br>
                                                                        <span style="color: Black">[</span><asp:Label ID="lblbuttonActionWorkflow1" Text="<%# Bind('ButtonAction') %>"
                                                                            runat="server"></asp:Label><span style="color: black"> ]</span>
                                                                    </td>
                                                                    <td align="center" width="65%">
                                                                        <asp:Label ForeColor="Blue" ID="Label3" Text="<%# Bind('SubmittedBy') %>" runat="server"></asp:Label>
                                                                        <span style="color: blue">[</span><asp:Label ForeColor="Blue" ID="lblloginName" Text="<%# Bind('LoginName') %>"
                                                                            runat="server"></asp:Label><span style="color: blue">]</span>
                                                                        <br>
                                                                        <asp:Label ID="lblbuttonActionWorkflow" ForeColor="Green" Text="<%# Bind('ButtonAction') %>"
                                                                            runat="server"></asp:Label><br>
                                                                        <asp:Label ForeColor="Blue" ID="Label1" Text="<%# Bind('Date') %>" runat="server"></asp:Label>
                                                                        <hr style="background-color: Black; height: 1px">
                                                                        <div style="width: 99%; white-space: normal; word-wrap: break-word; text-align: center;">
                                                                            Comments:
                                                                            <asp:Label ID="lblDate" Text="<%# Bind('Comments') %>" runat="server"></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr align="center" style="height: 40px;">
                                        <td id="Td1" class="Project_Sub_Heading">
                                            <asp:Label ID="lblactionlogs" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="Tr6" align="center" style="height: 40px;">
                                        <td align="left" bgcolor="#F1F1F1">
                                            <asp:GridView ID="grdviewAction" CssClass="myGridStyleActionLogs" Width="100%" runat="server"
                                                RowStyle-Wrap="false" HeaderStyle-BackColor="#1d558f" AutoGenerateColumns="false"
                                                OnRowDataBound="grdviewAdmin_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="left"
                                                        ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblbuttonAction" Text="<%# Bind('ButtonAction') %>" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action By" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="left"
                                                        ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label2" Text="<%# Bind('ByWhom') %>" runat="server"></asp:Label>:
                                                            <asp:Label ID="Label5" Text="<%# Bind('SubmittedBy') %>" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Date" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="left"
                                                        ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAction" Text="<%# Bind('Date') %>" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Comments" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="left"
                                                        ItemStyle-Width="55%" ItemStyle-Wrap="true">
                                                        <ItemTemplate>
                                                            <div style="text-align: left;">
                                                                <asp:Label ID="lblDate" Text="<%# Bind('Comments') %>" runat="server"></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <table>
                    <tr align="center">
                        <td align="center">
                            <asp:Button ID="btnSixSigmaSave" runat="server" Text="Save" Height="30px" Width="90px"
                                Font-Names="Calibri" Font-Size="12pt" OnClientClick="javascript:return FreezeScreen();"
                                OnClick="btnSixSigmaSave_Click" />
                            <asp:Button ID="btnUnlockform" runat="server" Width="200px" Height="30px" Text="Unlock form"
                                OnClientClick="return ProjectComments('btnUnlockform')" Font-Names="Calibri"
                                Font-Size="12pt" />
                            <asp:Button ID="btnlockform" runat="server" Width="200px" Height="30px" Text="Lock form"
                                OnClientClick="return ProjectComments('btnlockform')" Font-Names="Calibri" Font-Size="12pt" />
                            <asp:Button ID="btnEditCompleted" runat="server" Width="200px" Height="30px" Text="Edit Completed"
                                OnClientClick="return ProjectComments('btnEditCompleted')" Font-Names="Calibri"
                                Font-Size="12pt" />
                            <asp:Button ID="btnProjectAuthorization" runat="server" Width="240px" Height="30px"
                                Text="Request Project Authorization" OnClientClick="return ProjectComments('btnProjectAuthorization')"
                                Font-Names="Calibri" Font-Size="12pt" />
                            <asp:Button ID="btnSixSigmaClose" runat="server" Text="Close" Height="30px" Width="90px"
                                Font-Names="Calibri" Font-Size="12pt" OnClick="Click_Close" />
                            <asp:Button ID="btnSponsorApproval" runat="server" Width="200px" Height="30px" Text="Sponsor Approval"
                                OnClientClick="return ProjectComments('btnSponsorApproval')" Font-Names="Calibri"
                                Font-Size="12pt" />
                            <asp:Button ID="btnBBApproval" runat="server" Width="200px" Height="30px" Text="BB Approval"
                                OnClientClick="return ProjectComments('btnBBApproval')" Font-Names="Calibri"
                                Font-Size="12pt" />
                            <asp:Button ID="btnReturnProjectLead" runat="server" Width="200px" Height="30px"
                                Text="Return to Project Lead" OnClientClick="return ProjectComments('btnReturnProjectLead')"
                                Font-Names="Calibri" Font-Size="12pt" />
                        </td>
                    </tr>
                </table>
            </div>
            <table id="TableRC" width="1040px" class="tableCss noBorder" style="display: none">
                <tr id="TrReason" runat="server" align="center" style="height: 30px;">
                    <td style="padding-left: 10px;" align="left" bgcolor="#000000">
                        <span id="CommentsReason" style="color: White; font-size: 16pt; font-family: 'Calibri';"
                            runat="server">Comments</span>
                    </td>
                </tr>
                <tr id="Troriginator" runat="server" align="center" style="height: 10px;">
                    <td align="center">
                        <asp:TextBox ID="txtreturntooriginator" TextMode="MultiLine" CssClass="ECTS_label"
                            Height="100px" Width="99%" MaxLength="800" Rows="6" onkeydown="limitText(800);"
                            onkeyup="limitText(800);" runat="server" Font-Names="Calibri" Font-Size="12pt"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td bgcolor="#F1F1F1" align="right">
                        <font size="1">(Maximum characters: 800)&nbsp;&nbsp;You have
                            <input type="text" runat="server" id="txtCountdown" size="3" value="800" readonly="readonly" />
                            characters left.</font>
                    </td>
                </tr>
                <tr id="TrReasonError" runat="server" align="center" style="height: 40px; display: none">
                    <td style="padding-left: 10px;" align="left" bgcolor="#F1F1F1">
                        <asp:Label ID="TrReasonErrorErrorLabel" runat="server" Font-Size="12pt" Font-Names="Calibri"
                            ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td id="TdotherUserLabel" runat="server" style="display: none">
                        <asp:Label ID="lblErrormessageotherUser" runat="server" Font-Size="14pt" Font-Names="Calibri"
                            ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="ECTS_label">
                        <asp:Button ID="Bt_Ok_Click_On_Reject" runat="server" Text="OK" OnClientClick="javascript:return FreezeScreen();"
                            OnClick="Bt_CommentsOk_Click" />
                        <asp:Button ID="Bt_Cancel_On_Reject" runat="server" Text="Cancel" OnClientClick="javascript:return changeCommentsViews('cancel');" />
                    </td>
                </tr>
            </table>
        </center>
        <asp:HiddenField ID="ProjectCommentsFlag" runat="server" />
        <asp:HiddenField ID="SelectedTab" runat="server" />
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:HiddenField ID="Hidden1" runat="server" />
        <asp:HiddenField ID="Hidden2" runat="server" />
        <asp:HiddenField ID="hiddenMinimumDate" runat="server" />
        <asp:HiddenField ID="hiddenFieldQuadCharts" runat="server" />
        <asp:HiddenField ID="hdnGetPopupReturnValue" runat="server" />
        <asp:HiddenField ID="hdnGetPopupReturnValue1" runat="server" />
        <asp:HiddenField ID="HiddenLanguage" runat="server" />
    </contenttemplate>
</asp:UpdatePanel>
